using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class TileLayerMapper
	{
		private bool m_success;

		private Map m_map;

		private MapControl m_coreMap;

		private Dictionary<string, MapTileLayer> m_mapTileLayers;

		internal TileLayerMapper(Map map, MapControl coreMap)
		{
			m_map = map;
			m_coreMap = coreMap;
			m_mapTileLayers = new Dictionary<string, MapTileLayer>();
			m_coreMap.mapCore.LoadTilesHandler = LoadTilesHandler;
			m_coreMap.mapCore.SaveTilesHandler = SaveTilesHandler;
		}

		internal void AddLayer(MapTileLayer mapTileLayer)
		{
			m_mapTileLayers.Add(mapTileLayer.Name, mapTileLayer);
			m_coreMap.Layers[mapTileLayer.Name].TileSystem = GetTileSystem(mapTileLayer);
			m_coreMap.Layers[mapTileLayer.Name].UseSecureConnectionForTiles = GetUseSecureConnection(mapTileLayer);
		}

		private bool GetUseSecureConnection(MapTileLayer mapTileLayer)
		{
			ReportBoolProperty useSecureConnection = mapTileLayer.UseSecureConnection;
			if (useSecureConnection == null)
			{
				return false;
			}
			if (!useSecureConnection.IsExpression)
			{
				return useSecureConnection.Value;
			}
			return mapTileLayer.Instance.UseSecureConnection;
		}

		private TileSystem GetTileSystem(MapTileLayer mapTileLayer)
		{
			ReportEnumProperty<MapTileStyle> tileStyle = mapTileLayer.TileStyle;
			MapTileStyle mapTileStyle = MapTileStyle.Road;
			if (tileStyle != null)
			{
				mapTileStyle = (tileStyle.IsExpression ? mapTileLayer.Instance.TileStyle : tileStyle.Value);
			}
			switch (mapTileStyle)
			{
			case MapTileStyle.Aerial:
				return TileSystem.VirtualEarthAerial;
			case MapTileStyle.Hybrid:
				return TileSystem.VirtualEarthHybrid;
			default:
				return TileSystem.VirtualEarthRoad;
			}
		}

		private bool Embedded(MapTileLayer mapTileLayer)
		{
			return mapTileLayer.MapTiles != null;
		}

		private MapTileLayer GetLayer(string layerName)
		{
			m_mapTileLayers.TryGetValue(layerName, out MapTileLayer value);
			Global.Tracer.Assert(value != null, "null != tileLayer");
			return value;
		}

		private System.Drawing.Image[,] LoadTilesHandler(Layer layer, string[,] tileUrls)
		{
			System.Drawing.Image[,] array = null;
			int num = tileUrls.GetUpperBound(0) + 1;
			int num2 = tileUrls.GetUpperBound(1) + 1;
			MapTileLayer layer2 = GetLayer(layer.Name);
			try
			{
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						System.Drawing.Image image = (!Embedded(layer2)) ? GetSnapshotTile(layer2, tileUrls[i, j]) : GetEmbeddedTile(layer2, tileUrls[i, j]);
						if (image == null)
						{
							DisposeTiles(array, num, num2);
							return null;
						}
						if (array == null)
						{
							array = new System.Drawing.Image[num, num2];
						}
						array[i, j] = image;
					}
				}
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				DisposeTiles(array, num, num2);
				return null;
			}
			m_success = (array != null);
			return array;
		}

		private void DisposeTiles(System.Drawing.Image[,] tiles, int row, int col)
		{
			if (tiles == null)
			{
				return;
			}
			for (int i = 0; i < row; i++)
			{
				for (int j = 0; j < col; j++)
				{
					if (tiles[i, j] != null)
					{
						tiles[i, j].Dispose();
					}
				}
			}
		}

		private System.Drawing.Image GetSnapshotTile(MapTileLayer mapTileLayer, string url)
		{
			string mimeType;
			Stream tileData = mapTileLayer.Instance.GetTileData(url, out mimeType);
			if (tileData == null)
			{
				return null;
			}
			return System.Drawing.Image.FromStream(tileData);
		}

		private System.Drawing.Image GetEmbeddedTile(MapTileLayer mapTileLayer, string url)
		{
			foreach (MapTile mapTile in mapTileLayer.MapTiles)
			{
				if (mapTile.Name == url)
				{
					using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(mapTile.TileData)))
					{
						return System.Drawing.Image.FromStream(stream);
					}
				}
			}
			return null;
		}

		private void SaveTilesHandler(Layer layer, string[,] tileUrls, System.Drawing.Image[,] tileImages)
		{
			MapTileLayer layer2 = GetLayer(layer.Name);
			if (Embedded(layer2) || m_success)
			{
				return;
			}
			int num = tileUrls.GetUpperBound(0) + 1;
			int num2 = tileUrls.GetUpperBound(1) + 1;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					string url = tileUrls[i, j];
					System.Drawing.Image image = tileImages[i, j];
					using (MemoryStream memoryStream = new MemoryStream())
					{
						image.Save(memoryStream, ImageFormat.Png);
						if (layer2.Instance.GetTileData(url, out string _) == null)
						{
							layer2.Instance.SetTileData(url, memoryStream.ToArray(), null);
						}
					}
				}
			}
		}
	}
}
