// <copyright company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
var checkBoxCount;
var checkBoxId;
var checkBoxHead;

// Context menu
var _divContextMenu; // The container for the context menu
var _selectedIdHiddenField; // The id of the item that opened th context menu
var _timeOutLimit = 3000; // How long the context menu stays for after the cursor in no longer over it
var _timeOutTimer; // The timout for the context menu
var _itemSelected = false;
var _mouseOverContext = false; // If the mouse is over the context menu
var _contextMenusIds; // The array of the diffrent context menus
var _fadeTimeouts; // The array of timouts used for the fade effect
var _onLink = false; // If the user is over a name link
var _selectedItemId;
var _tabFocusedItem = '';
var _mouseOverItem = '';
var _unselectedItemStyle;
var _currentContextMenuId;  // ID of currently displayed context menu
var _currentMenuItemId = null;     // ID of currently selected context menu item

// Search bar
var _searchTextBoxID;
var _defaultSearchValue; // The value that the box defaults to.

function ToggleItem(itemId) {
    var item = document.getElementById(itemId);
    if (item.style.display == 'none')
        item.style.display = 'inline';
    else
        item.style.display = 'none';
}

function ToggleButtonImage(image1ID, image2ID) {
    var image1 = document.getElementById(image1ID);
    var image2 = document.getElementById(image2ID);
    if (image1.style.display == 'none') {
        image1.style.display = 'inline-block';
        image2.style.display = 'none';
    }
    else {
        image1.style.display = 'none';
        image2.style.display = 'inline-block';
    }
}

function SetFocus(id) {
    var obj = document.getElementById(id);
    if (obj != null && !obj.disabled)
        obj.focus();
}

// Validates that an extension has been selected
function ValidateDropDownSelection(source, args) {
    var obj = document.getElementById(source.controltovalidate);

    if (obj.options[0].selected && !obj.disabled)
        args.IsValid = false;
    else
        args.IsValid = true;
}

/// selectAll
/// selects all the checkBoxes with the given id
function selectAll() {
    var i;
    var id;
    var checked = checkBoxHead.checked;
    for (i = 0; i < checkBoxCount; i++) {
        id = checkBoxId + i;
        document.getElementById(id).checked = checked;
    }
}

/// onSglCheck
/// performs actions when a single checkBox is checked or unchecked
/// cb -> the checkBox generating the event
/// topId -> id of the "select all" checkBox
function onSglCheck() {
    // uncheck the top checkBox
    checkBoxHead.checked = false;
}

/// ToggleButton
/// Toggle a buttons enable state
function ToggleButton(id, disabled) {
    if (document.getElementById(id) != null)
        document.getElementById(id).disabled = disabled;
}

function ToggleValidator(id, enabled) {
    document.getElementById(id).enabled = enabled;
}

function SetCbVars(cbid, count, cbh) {
    checkBoxCount = count;
    checkBoxId = cbid;
    checkBoxHead = cbh;
}

/// Check to see if any check boxes should disable 
/// a control
/// cbid -> id prefix of the checkBoxes
/// cbCount -> total checkBoxes to check
/// hidden -> input to look for
/// display -> control to disable
function CheckCheckBoxes(cbid, hidden, display) {
    var i;
    var id;
    var disable;

    disable = false;
    for (i = 0; i < checkBoxCount; i++) {
        id = cbid + i;
        if (document.getElementById(id).checked) {
            id = hidden + id;
            if (document.getElementById(id) != null) {
                disable = true;
                break;
            }
        }
    }

    ToggleButton(display, disable);
}

function HiddenCheckClickHandler(hiddenID, promptID, promptStringID) {
    var hiddenChk = document.getElementById(hiddenID);
    var promptChk = document.getElementById(promptID);

    // prompt should be in opposite state of hidden
    promptChk.checked = !hiddenChk.checked;
}

function validateSaveRole(source, args) {
    var i;
    var id;
    var c = 0;
    for (i = 0; i < checkBoxCount; i++) {
        id = checkBoxId + i;
        if (document.getElementById(id).checked) c++;
    }
    if (0 == c)
        args.IsValid = false;
    else
        args.IsValid = true;
}

/// Pad an integer less then 10 with a leading zero
function PadIntWithZero(val) {
    var s = val.toString();

    if (val < 10 && val >= 0) {
        if (s.length == 1)
            s = "0" + s;
        else if (s.length > 2)
            s = s.substring(s.length - 2, s.length);
    }

    return s;
}

/// Pad the contents of an input with leading zeros if necesarry
function PadInputInteger(id) {
    document.getElementById(id).value = PadIntWithZero(document.getElementById(id).value);
}

/// Given a number of checked items, confirm their changes
/// return true if user selects OK
function doCmChanges(checkedCount, confirmText) {
    if (checkedCount == 0)
        return false;
    // if only one item was selected, we don't need to confirm
    if (checkedCount == 1)
        return true;
    return confirm(confirmText);
}

function confirmChanges(confirmText) {
    return doCmChanges(getChkCount(), confirmText);
}

/// text of confirmation popup when a single item is selected for deletion
/// e.g. "Are you sure you want to delete this item"
var confirmSingle;

/// text of confirmation popup when multiple items are selected for deletion
/// e.g. "Are you sure you want to delete these items"
var confirmMultiple;
function SetDeleteTxt(single, multiple) {
    confirmSingle = single;
    confirmMultiple = multiple;
}

/// doCmDel: DoConfirmDelete
/// Given a number of checked items, confirm their deletion
/// return true if OK was clicked; false otherwise
function doCmDel(checkedCount) {
    var confirmTxt = confirmSingle;
    if (checkedCount == 0)
        return false;

    if (checkedCount > 1)
        confirmTxt = confirmMultiple;
    return confirm(confirmTxt);
}

/// on non-Netscape browsers, confirm deletion of 0 or more items
function confirmDelete() {
    return doCmDel(getChkCount());
}

/// confirm deletion of policies
function confirmDeletePlcies(alertString) {
    var count = getChkCount();
    if (count >= checkBoxCount) {
        alert(alertString);
        return false;
    }
    return doCmDel(count);
}

/// counts whether 0, 1, or more than 1 checkboxes are checked
/// returns 0, 1, or 2
function getChkCount() {
    var checkedCount = 0;
    for (i = 0; i < checkBoxCount && checkedCount < 2; i++) {
        if (document.getElementById(checkBoxId + i).checked) {
            checkedCount++;
        }
    }
    return checkedCount;
}

function ToggleButtonBasedOnCheckBox(checkBoxId, toggleId, reverse) {
    var chkb = document.getElementById(checkBoxId);
    if (chkb != null) {
        if (chkb.checked == true)
            ToggleButton(toggleId, reverse); // enable if reverse == false
        else
            ToggleButton(toggleId, !reverse); // disable if reverse == false
    }
}

function ToggleButtonBasedOnCheckBoxWithOverride(checkBoxId, toggleId, overrideToDisabled, reverse) {
    if (overrideToDisabled == true)
        ToggleButton(toggleId, true); // disable
    else
        ToggleButtonBasedOnCheckBox(checkBoxId, toggleId, reverse);
}

function ToggleButtonBasedOnCheckBoxes(checkBoxId, checkboxId2, toggleId) {
    var chkb = document.getElementById(checkBoxId);
    if (chkb != null) {
        if (chkb.checked == true)
            ToggleButtonBasedOnCheckBox(checkboxId2, toggleId, false);
        else
            ToggleButton(toggleId, true); // disable
    }

}

function ToggleButtonBasedOnCheckBoxesWithOverride(checkBoxId, checkboxId2, toggleId, overrideToDisabled) {
    if (overrideToDisabled == true)
        ToggleButton(toggleId, true); // disable
    else
        ToggleButtonBasedOnCheckBoxes(checkBoxId, checkboxId2, toggleId);
}

function ToggleValidatorBasedOnCheckBoxWithOverride(checkBoxId, toggleId, overrideToDisabled, reverse) {
    if (overrideToDisabled == true)
        ToggleValidator(toggleId, false);
    else {
        var chkb = document.getElementById(checkBoxId);
        if (chkb != null) {
            ToggleValidator(toggleId, chkb.checked != reverse);
        }
    }
}

function ToggleValidatorBasedOnCheckBoxesWithOverride(checkBoxId, checkBoxId2, toggleId, overrideToDisabled, reverse) {
    if (overrideToDisabled == true)
        ToggleValidator(toggleId, false);
    else {
        var chkb = document.getElementById(checkBoxId);
        if (chkb != null) {
            if (chkb.checked == reverse)
                ToggleValidator(toggleId, false);
            else
                ToggleValidatorBasedOnCheckBoxWithOverride(checkBoxId2, toggleId, overrideToDisabled, reverse);
        }
    }
}

function CheckButton(buttonID, shouldCheck) {
    document.getElementById(buttonID).checked = shouldCheck;
}

function EnableMultiButtons(prefix) {
    // If there are no multibuttons, there is no reason to iterate the
    // list of checkboxes.
    if (checkBoxCount == 0 || multiButtonList.length == 0)
        return;

    var enableMultiButtons = false;
    var multipleCheckboxesSelected = false;

    // If the top level check box is checked, we know the state of all
    // of the checkboxes
    var headerCheckBox = document.getElementById(prefix + "ch");
    if (headerCheckBox != null && headerCheckBox.checked) {
        enableMultiButtons = true;
        multipleCheckboxesSelected = checkBoxCount > 1;
    }
    else {
        // Look at each checkbox.  If any one of them is checked,
        // enable the multi buttons.
        var foundOneChecked = false;
        var i;
        for (i = 0; i < checkBoxCount; i++) {
            var checkBox = document.getElementById(prefix + 'cb' + i);
            if (checkBox.checked) {               
                if (foundOneChecked) {
                    multipleCheckboxesSelected = true;
                    break;
                }
                else {
                    enableMultiButtons = true;
                    foundOneChecked = true;
                }
            }
        }
    }

    // Enable/disable each of the multi buttons	
    var j;
    for (j = 0; j < multiButtonList.length; j++) {
        var button = document.getElementById(multiButtonList[j]);
        if (button.allowMultiSelect)
            button.disabled = !enableMultiButtons;
        else
            button.disabled = !enableMultiButtons || multipleCheckboxesSelected;
    }
}

//function ShadowCopyPassword(suffix)
function MarkPasswordFieldChanged(suffix) {
    if (event.propertyName == "value") {
        var pwdField = document.getElementById("ui_txtStoredPwd" + suffix);
        //var shadowField = document.getElementById("ui_shadowPassword" + suffix);
        var shadowChanged = document.getElementById("ui_shadowPasswordChanged" + suffix);

        // Don't shadow copy during initialization
        if (pwdField.IsInit) {
            //shadowField.value = pwdField.value;
            //pwdField.UserEnteredPassword = "true";
            shadowChanged.value = "true";

            // Update validator state (there is no validator on the data driven subscription page)
            var validator = document.getElementById("ui_validatorPassword" + suffix)
            if (validator != null)
                ValidatorValidate(validator);
        }
    }
}

function InitDataSourcePassword(suffix) {
    var pwdField = document.getElementById("ui_txtStoredPwd" + suffix);
    var shadowChanged = document.getElementById("ui_shadowPasswordChanged" + suffix);
    //	var shadowField = document.getElementById("ui_shadowPassword" + suffix);
    var storedRadioButton = document.getElementById("ui_rdoStored" + suffix);
    var pwdValidator = document.getElementById("ui_validatorPassword" + suffix);

    pwdField.IsInit = false;

    // Initialize the field to the shadow value (for when the user clicks back/forward)
    // Or to a junk initial value.
    if (pwdValidator != null && storedRadioButton.checked) {
        /*		if (shadowField.value.length > 0)
        pwdField.value = shadowField.value;
        else*/
        pwdField.value = "********";
    }
    else
        shadowChanged.value = "true"; // shadowChanged will be ignored if the page is submitted without storedRadioButton.checked

    // Now that the initial value is set, track changes to the password field
    pwdField.IsInit = true;

    // There is no validator on the data driven subscription page (no stored radio button either)
    if (pwdValidator != null)
        ValidatorValidate(pwdValidator);
}

function SetNeedPassword(suffix) {
    // Set a flag indicating that we need the password
    var pwdField = document.getElementById("ui_txtStoredPwd" + suffix);
    pwdField.NeedPassword = "true";

    // Make the validator visible
    ValidatorValidate(document.getElementById("ui_validatorPassword" + suffix));
}

function UpdateValidator(src, validatorID) {
    if (src.checked) {
        var validator = document.getElementById(validatorID);
        ValidatorValidate(validator);
    }
}

function ReEnterPasswordValidation(source, args) // source = validator
{
    var validatorIdPrefix = "ui_validatorPassword"
    var suffix = source.id.substr(validatorIdPrefix.length, source.id.length - validatorIdPrefix.length);

    var storedRadioButton = document.getElementById("ui_rdoStored" + suffix);
    var pwdField = document.getElementById("ui_txtStoredPwd" + suffix);
    var shadowChanged = document.getElementById("ui_shadowPasswordChanged" + suffix);

    var customDataSourceRadioButton = document.getElementById("ui_rdoCustomDataSource" + suffix);
    var isCustomSelected = true;
    if (customDataSourceRadioButton != null)
        isCustomSelected = customDataSourceRadioButton.checked;

    if (!isCustomSelected || 					// If the custom (vs shared) data source radio button exists and is not selected, we don't need the pwd.
	    storedRadioButton.checked == false || 	// If the data source is not using stored credentials, we don't need the password
	    pwdField.UserEnteredPassword == "true" || // If the password has changed, we don't need to get it from the user
	    pwdField.NeedPassword != "true" || 		// If no credentials have changed, we don't need the password
	    shadowChanged.value == "true")				// If the user has typed a password
        args.IsValid = true;
    else
        args.IsValid = false;
}

function ValidateDataSourceSelected(source, args) {
    var validatorIdPrefix = "ui_sharedDSSelectedValidator"
    var suffix = source.id.substr(validatorIdPrefix.length, source.id.length - validatorIdPrefix.length);

    var sharedRadioButton = document.getElementById("ui_rdoSharedDataSource" + suffix);
    var hiddenField = document.getElementById("ui_hiddenSharedDS" + suffix);

    args.IsValid = (sharedRadioButton != null && !sharedRadioButton.checked) || hiddenField.value != "NotSelected";
}



/**************************************************************************/
// MultiValueParamClass
function MultiValueParamClass(thisID, visibleTextBoxID, floatingEditorID, floatingIFrameID, paramObject,
                              hasValidValues, allowBlank, doPostbackOnHide, postbackScript) {
    this.m_thisID = thisID;
    this.m_visibleTextBoxID = visibleTextBoxID;
    this.m_floatingEditorID = floatingEditorID;
    this.m_floatingIFrameID = floatingIFrameID;
    this.m_paramObject = paramObject;
    this.m_hasValidValues = hasValidValues;
    this.m_allowBlank = allowBlank;
    this.m_doPostbackOnHide = doPostbackOnHide;
    this.m_postbackScript = postbackScript;

    this.UpdateSummaryString();
}

function ToggleVisibility() {
    var floatingEditor = GetControl(this.m_floatingEditorID);
    if (floatingEditor.style.display != "inline")
        this.Show();
    else
        this.Hide();
}
MultiValueParamClass.prototype.ToggleVisibility = ToggleVisibility;

function Show() {
    var floatingEditor = GetControl(this.m_floatingEditorID);
    if (floatingEditor.style.display == "inline")
        return;

    // Set the correct size of the floating editor - no more than
    // 150 pixels high and no less than the width of the text box
    var visibleTextBox = GetControl(this.m_visibleTextBoxID);
    if (this.m_hasValidValues) {
        if (floatingEditor.offsetHeight > 150)
            floatingEditor.style.height = 150;
        floatingEditor.style.width = visibleTextBox.offsetWidth;
    }

    var newEditorPosition = this.GetNewFloatingEditorPosition();
    floatingEditor.style.left = newEditorPosition.Left;
    floatingEditor.style.top = newEditorPosition.Top;
    floatingEditor.style.display = "inline";

    var floatingIFrame = GetControl(this.m_floatingIFrameID);
    floatingIFrame.style.left = floatingEditor.style.left;
    floatingIFrame.style.top = floatingEditor.style.top;
    floatingIFrame.style.width = floatingEditor.offsetWidth;
    floatingIFrame.style.height = floatingEditor.offsetHeight;
    floatingIFrame.style.display = "inline";

    // If another multi value is open, close it first
    if (this.m_paramObject.ActiveMultValue != this && this.m_paramObject.ActiveMultiValue != null)
        ControlClicked(this.m_paramObject.id);
    this.m_paramObject.ActiveMultiValue = this;

    if (floatingEditor.childNodes[0].focus) floatingEditor.childNodes[0].focus();
    this.StartPolling();
}
MultiValueParamClass.prototype.Show = Show;

function Hide() {
    var floatingEditor = GetControl(this.m_floatingEditorID);
    var floatingIFrame = GetControl(this.m_floatingIFrameID);

    // Hide the editor
    floatingEditor.style.display = "none";
    floatingIFrame.style.display = "none";
    this.UpdateSummaryString();

    if (this.m_doPostbackOnHide)
        eval(this.m_postbackScript);

    // Check that the reference is still us in case event ordering
    // caused another multivalue to click open
    if (this.m_paramObject.ActiveMultiValue == this)
        this.m_paramObject.ActiveMultiValue = null;
}
MultiValueParamClass.prototype.Hide = Hide;

function GetNewFloatingEditorPosition() {
    // Make the editor visible
    var visibleTextBox = GetControl(this.m_visibleTextBoxID);
    var textBoxPosition = GetObjectPosition(visibleTextBox);

    return { Left: textBoxPosition.Left, Top: textBoxPosition.Top + visibleTextBox.offsetHeight };
}
MultiValueParamClass.prototype.GetNewFloatingEditorPosition = GetNewFloatingEditorPosition;

function UpdateSummaryString() {
    var summaryString;

    if (this.m_hasValidValues)
        summaryString = GetValueStringFromValidValueList(this.m_floatingEditorID);
    else
        summaryString = GetValueStringFromTextEditor(this.m_floatingEditorID, false, this.m_allowBlank);

    var visibleTextBox = GetControl(this.m_visibleTextBoxID);
    visibleTextBox.value = summaryString;
}
MultiValueParamClass.prototype.UpdateSummaryString = UpdateSummaryString;

function StartPolling() {
    setTimeout(this.m_thisID + ".PollingCallback();", 100);
}
MultiValueParamClass.prototype.StartPolling = StartPolling;

function PollingCallback() {
    // If the editor isn't visible, no more events.
    var floatingEditor = GetControl(this.m_floatingEditorID);
    if (floatingEditor.style.display != "inline")
        return;

    // If the text box moved, something on the page resized, so close the editor
    var expectedEditorPos = this.GetNewFloatingEditorPosition();
    if (floatingEditor.style.left != expectedEditorPos.Left + "px" ||
        floatingEditor.style.top != expectedEditorPos.Top + "px") {
        this.Hide();
    }
    else {
        this.StartPolling();
    }
}
MultiValueParamClass.prototype.PollingCallback = PollingCallback;
/*****************************************************************************/

function GetObjectPosition(obj) {
    var totalTop = 0;
    var totalLeft = 0;
    while (obj != document.body) {
        // Add up the position
        totalTop += obj.offsetTop;
        totalLeft += obj.offsetLeft;

        // Prepare for next iteration
        obj = obj.offsetParent;
    }

    totalTop += obj.offsetTop;
    totalLeft += obj.offsetLeft;

    return { Left: totalLeft, Top: totalTop };
}

function GetValueStringFromTextEditor(floatingEditorID, asRaw, allowBlank) {
    var span = GetControl(floatingEditorID);
    var editor = span.childNodes[0];

    var valueString = editor.value;

    // Remove the blanks
    if (!allowBlank) {
        // Break down the text box string to the individual lines
        var valueArray = valueString.split("\r\n");

        var delimiter;
        if (asRaw)
            delimiter = "\r\n";
        else
            delimiter = ", ";

        var finalValue = "";
        for (var i = 0; i < valueArray.length; i++) {
            // If the string is non-blank, add it
            if (valueArray[i].length > 0) {
                if (finalValue.length > 0)
                    finalValue += delimiter;
                finalValue += valueArray[i];
            }
        }

        return finalValue;
    }
    else {
        if (asRaw)
            return valueString;
        else
            return valueString.replace(/\r\n/g, ", ");
    }
}

function GetValueStringFromValidValueList(editorID) {
    var valueString = "";

    // Get the table
    var div = GetControl(editorID);
    var table = div.childNodes[0];
    if (table.nodeName != "TABLE")  // Skip whitespace if needed
        table = div.childNodes[1];
    
    // If there is only one element, it is a real value, not the select all option
    var startIndex = 0;
    if (table.rows.length > 1)
        startIndex = 1;
            
    for (var i = startIndex; i < table.rows.length; i++)
    {
        // Get the first cell of the row
        var firstCell = table.rows[i].cells[0];
        var span = firstCell.childNodes[0];

        var checkBox = span.childNodes[0];
        var label = span.childNodes[1];

        if (checkBox.checked) {
            if (valueString.length > 0)
                valueString += ", ";
            valueString += label.firstChild.nodeValue;
        }
    }

    return valueString;
}

function MultiValidValuesSelectAll(src, editorID)
{
    // Get the table
    var div = GetControl(editorID);
    var table = div.childNodes[0];
    if (table.nodeName != "TABLE")
        table = div.childNodes[1];
    
    for (var i = 1; i < table.rows.length; i++)
    {
        // Get the first cell of the row
        var firstCell = table.rows[i].cells[0];
        var span = firstCell.childNodes[0];
        
        var checkBox = span.childNodes[0];
        checkBox.checked = src.checked;
    }
}

function ValidateMultiValidValue(editorID, errMsg)
{
    var summaryString = GetValueStringFromValidValueList(editorID);
    var isValid = summaryString.length > 0;
    if (!isValid)
        alert(errMsg)

    return isValid;
}

function ValidateMultiEditValue(editorID, errMsg) {
    // Need to check for a value specified.  This code only runs if not allow blank.
    // GetValueStringFromTextEditor filters out blank strings.  So if it was all blank,
    // the final string will be length 0
    var summaryString = GetValueStringFromTextEditor(editorID, true, false)

    var isValid = false;
    if (summaryString.length > 0)
        isValid = true;

    if (!isValid)
        alert(errMsg);

    return isValid;
}

function GetControl(controlID) {
    var control = document.getElementById(controlID);
    if (control == null)
        alert("Unable to locate control: " + controlID);

    return control;
}

function ControlClicked(formID) {
    var form = GetControl(formID);

    if (form.ActiveMultiValue != null)
        form.ActiveMultiValue.Hide();
}

// --- Context Menu ---

// This function is called in the onload event of the body.
// It hooks the context menus up to the Javascript code.
//      divContextMenuId, is the id of the div that contains the context menus
//      selectedIdHiddenFieldId, is the id of the field used to post back the name of the item clicked
//      contextMenusIds, is an array of the ids of the context menus
//      searchTextBox ID, is the id of the search box
//      defaultSearchValue. the value the search box has by default
function InitContextMenu(divContextMenuId, selectedIdHiddenFieldId, contextMenusIds, searchTextBoxID, defaultSearchValue ) {

    ResetSearchBar( searchTextBoxID, defaultSearchValue );

    _divContextMenu = document.getElementById(divContextMenuId);
    _selectedIdHiddenField = document.getElementById(selectedIdHiddenFieldId);
    _contextMenusIds = contextMenusIds;
    _divContextMenu.onmouseover = function() { _mouseOverContext = true; };
    _divContextMenu.onmouseout = function() {
        if (_mouseOverContext == true) {
            _mouseOverContext = false;

            if (_timeOutTimer == null) {
                _timeOutTimer = setTimeout(TimeOutAction, _timeOutLimit);
            }
        }

    };
    
    document.body.onmousedown = ContextMouseDown;
    AddKeyDownListener();
}

// This handler stops bubling when arrow keys Up or Down pressed to prevent scrolling window
function KeyDownHandler(e)
{	
    // Cancel window scrolling only when menu is opened
    if(_currentContextMenuId == null)
    {
        return true;
    }
    
	if(!e) 
	{ 
	    e = window.event; 
	}
	
	var key = e.keyCode;

	if(key == 38 || key == 40) 
	{ 
	    return false; 
	}
	else 
	{ 
	    return true; 
	}
}

function AddKeyDownListener()
{
	if(document.addEventListener)
	{
		document.addEventListener('keydown', KeyDownHandler, false);
	}
	else
	{
		document.onkeydown = KeyDownHandler;
	}
}

// This function starts the context menu timeout process
function TimeOutAction() {
    if (_mouseOverContext == false) {

        UnSelectedMenuItem()
    }
    _timeOutTimer = null;
}

// This function is called when a name tag is clicked, it displays the contextmenu for a given item.
function Clicked(event, contextMenuId) {

    if (!_onLink) {
        
        ClearTimeouts();
        SelectContextMenuFromColletion(contextMenuId);

        _itemSelected = true;

        // **Cross browser compatibility code**
        // Some browsers will not pass the event so we need to get it from the window instead.
        if (event == null)
            event = window.event;

        var selectedElement = event.target != null ? event.target : event.srcElement;
        var outerTableElement = GetOuterElementOfType(selectedElement, 'table');
        var elementPosition = GetElementPosition(outerTableElement);

        _selectedItemId = outerTableElement.id;
        _selectedIdHiddenField.value = outerTableElement.getAttribute('value');
        outerTableElement.className = "msrs-SelectedItem";

        ResetContextMenu();

        var contextMenuHeight = _divContextMenu.offsetHeight;
        var contextMenuWidth = _divContextMenu.offsetWidth;

        var boxHeight = outerTableElement.offsetHeight;
        var boxWidth = outerTableElement.offsetWidth;
        var boxXcoordinate = elementPosition.left;
        var boxYcooridnate = elementPosition.top;

        var pageWidth = 0, pageHeight = 0;
        // **Cross browser compatibility code**
        if (typeof (window.innerWidth) == 'number') {
            //Non-IE
            pageWidth = window.innerWidth;
            pageHeight = window.innerHeight;
        } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
            //IE 6+ in 'standards compliant mode'
            pageWidth = document.documentElement.clientWidth;
            pageHeight = document.documentElement.clientHeight;
        } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
            //IE 4 compatible
            pageWidth = document.body.clientWidth;
            pageHeight = document.body.clientHeight;
        }

        // **Cross browser compatibility code**
        var iebody = (document.compatMode && document.compatMode != "BackCompat") ? document.documentElement : document.body
        var pageXOffSet = document.all ? iebody.scrollLeft : pageXOffset
        var pageYOffSet = document.all ? iebody.scrollTop : pageYOffset

        _divContextMenu.style.left = SetContextMenuHorizonatalPosition(pageWidth, pageXOffSet, boxXcoordinate, contextMenuWidth, boxWidth) + 'px';
        _divContextMenu.style.top = SetContextMenuVerticalPosition(pageHeight, pageYOffSet, boxYcooridnate, contextMenuHeight, boxHeight) + 'px';

        ChangeOpacityForElement(100, _divContextMenu.id);

        document.getElementById(_currentContextMenuId).firstChild.focus();
    }
}

// ***********************************
// Context menu keyboard navigation
// ***********************************

// Opens context menu via keyboard.  Context menu
// is opened by selecting an item and pressing
// Alt + Down.
function OpenMenuKeyPress(e, contextMenuId)
{
    // Alt key was pressed
    if (e.altKey)
    {
        var keyCode;

        if (window.event)
            keyCode = e.keyCode;
        else
            keyCode = e.which;

        // Down key was pressed
        if (keyCode == 40)
        {
            // Open context menu.
            Clicked(event, contextMenuId);

            // Highlight the first selectable item 
            // in the context menu.
            HighlightContextMenuItem(true);
        }
    }
}

// Performs keyboard navigation within
// opened context menu.
function NavigateMenuKeyPress(e)
{
    var keyCode;

    if (window.event)
        keyCode = e.keyCode;
    else
        keyCode = e.which;

    // Down key moves down to the next context menu item
    if (keyCode == 40)
    {            
        HighlightContextMenuItem(true);
    }

    // Up key moves up to the previous context menu item
    else if (keyCode == 38)
    {
        HighlightContextMenuItem(false);
    } 
    
    // Escape key closes context menu
    else if (keyCode == 27)
    {
        // Close context menu
        UnSelectedMenuItem();

        // Make sure focus is given to the catalog item
        // in the folder view.
        document.getElementById(_selectedItemId).focus();
    }        
}

// Highlights context menu item.
// Parameter:  highlightNext
// - If true, highlights menu item below current menu item.
//   If current menu item is the last item, wraps around and
//   highlights first menu item.
// - If false, highlights menu item above current menu item.
//   If current menu item is the first item, wraps around and
//   highlights last menu item.
function HighlightContextMenuItem(highlightNext)
{
    var contextMenu = document.getElementById(_currentContextMenuId);
    var table = GetLastChildByTagName(contextMenu, "table");
    var table_rows_len = table.rows.length;
    if (table_rows_len < 1)
        // not supposed to happen, but avoid infinite loop
        return;

    var currentMenuItemIndex = -1;
    if (_currentMenuItemId != null)
        currentMenuItemIndex = document.getElementById(_currentMenuItemId).parentNode.rowIndex;
        
    var index = currentMenuItemIndex;
    while (true)
    {
        if (highlightNext)
        {
            index++;

            // If the index is out of range,
            // reset it to the beginning
            if (index < 0 || index >= table_rows_len)
                index = 0;
        }
        else
        {
            index--;

            // If the index is out of range,
            // reset it to the end
            if (index < 0 || index >= table_rows_len)
                index = table_rows_len - 1;
        }

        // Each context menu item has an associated
        // group ID.  Make sure the table cell has a valid
        // group ID, otherwise it is not a menu item (e.g.
        // an underline separator).
        var current_td = GetLastChildByTagName(table.rows[index], "td");
        if (current_td.group >= 0)
        {
            FocusContextMenuItem(current_td.id, 'msrs-MenuUIItemTableHover', 'msrs-MenuUIItemTableCell');
            break;
        }

        // If we reach the orignal index, that means we looped
        // through all table cells and did not find a valid context
        // menu item.  In that case, stop searching.
        if (index == currentMenuItemIndex)
            break;
    }
}

// *** End keyboard navigation ***

// This function resets the context menus shape and size.
function ResetContextMenu() {
    _divContextMenu.style.height = 'auto';
    _divContextMenu.style.width = 'auto';
    _divContextMenu.style.overflowY = 'visible';
    _divContextMenu.style.overflowX = 'visible';
    _divContextMenu.style.overflow = 'visible';
    _divContextMenu.style.display = 'block';
}

// This function sets the horizontal position of the context menu.
// It also sets is the context menu has vertical scroll bars.
function SetContextMenuHorizonatalPosition(pageWidth, pageXOffSet, boxXcoordinate, contextMenuWidth, boxWidth) {

    var menuXCoordinate = boxXcoordinate + boxWidth - contextMenuWidth;
    var spaceRightBox = (pageWidth + pageXOffSet) - menuXCoordinate;
    var spaceLeftBox = menuXCoordinate - pageXOffSet;
    var returnValue;

    if ((contextMenuWidth < spaceRightBox) && (pageXOffSet < menuXCoordinate)) {
        returnValue = menuXCoordinate;
    }
    else if ((contextMenuWidth < spaceRightBox)) {
        returnValue = pageXOffSet;
    }
    else if (contextMenuWidth < spaceLeftBox) {
        returnValue = menuXCoordinate - (contextMenuWidth - (pageWidth + pageXOffSet - menuXCoordinate));
    }
    else {
        _divContextMenu.style.overflowX = "scroll";
        if (spaceLeftBox < spaceRightBox) {
            _divContextMenu.style.width = spaceRightBox;
            returnValue = pageXOffSet;
        }
        else {
            _divContextMenu.style.width = spaceLeftBox;
            returnValue = menuXCoordinate - (spaceLeftBox - (pageWidth + pageXOffSet - menuXCoordinate));
        }
    }

    return returnValue;
}

// This function sets the vertical position of the context menu.
// It also sets is the context menu has horizontal scroll bars.
function SetContextMenuVerticalPosition(pageHeight, pageYOffSet, boxYcooridnate, contextMenuHeight, boxHeight) {

    var spaceBelowBox = (pageHeight + pageYOffSet) - (boxYcooridnate + boxHeight);
    var spaceAboveBox = boxYcooridnate - pageYOffSet;
    var returnValue;

    if (contextMenuHeight < spaceBelowBox) {
        returnValue = (boxYcooridnate + boxHeight);
    }
    else if (contextMenuHeight < spaceAboveBox) {
        returnValue = (boxYcooridnate - contextMenuHeight);
    }
    else if (spaceBelowBox > spaceAboveBox) {
        _divContextMenu.style.height = spaceBelowBox;
        _divContextMenu.style.overflowY = "scroll";
        returnValue = (boxYcooridnate + boxHeight);
    }
    else {
        _divContextMenu.style.height = spaceAboveBox;
        _divContextMenu.style.overflowY = "scroll";
        returnValue = (boxYcooridnate - spaceAboveBox);
    }

    return returnValue;
}

// This function displays a context menu given its id and then hides the others
function SelectContextMenuFromColletion(contextMenuConfigString) {

    var contextMenuId = SplitContextMenuConfigString(contextMenuConfigString);
    
    for (i = 0; i < _contextMenusIds.length; i++) {
        var cm = document.getElementById(_contextMenusIds[i]);
        if (cm.id == contextMenuId) {
            cm.style.visibility = 'visible';
            cm.style.display = 'block';
            _currentContextMenuId = contextMenuId;
        }
        else {
            cm.style.visibility = 'hidden';
            cm.style.display = 'none';
        }
    }
}

function SplitContextMenuConfigString(contextMenuConfigString) {

    var contextMenuEnd = contextMenuConfigString.indexOf(":");
    var contextMenuId = contextMenuConfigString;
    var contextMenuHiddenItems;
    
    if (contextMenuEnd != -1)
    {
        contextMenuId = contextMenuConfigString.substr(0, contextMenuEnd);
    }
    
    var cm = document.getElementById(contextMenuId);

    var table = GetLastChildByTagName(cm, "table");
    var groupItemCount = []; // The items in each group
    var groupUnderlineId = []; // The Id's of the underlines.
    
    // Enable all menu items counting the number of groups, 
    // number of items in the groups and underlines for the groups as we go.
    for (i = 0; i < table.rows.length; i++) 
    {
        var current_td = GetLastChildByTagName(table.rows[i], "td");
        current_td.style.visibility = 'visible';
        current_td.style.display = 'block'

        if ((groupItemCount.length - 1) < current_td.group) {
            groupItemCount.push(1);
            groupUnderlineId.push(current_td.underline);
        }
        else {
            groupItemCount[current_td.group]++;
        }
        
        AlterVisibilityOfAssociatedUnderline(current_td, true)
    }

    // If hidden items are listed, remove them from the context menu
    if (contextMenuEnd != -1) 
    {            
        contextMenuHiddenItems = contextMenuConfigString.substr((contextMenuEnd + 1), (contextMenuConfigString.length - 1)).split("-");
        var groupsToHide = groupItemCount;
         
        // Hide the hidden items          
        for (i = 0; i < contextMenuHiddenItems.length; i++) 
        {           
            var item = document.getElementById(contextMenuHiddenItems[i]);

            item.style.visibility = 'hidden';
            item.style.display = 'none'
            
            groupsToHide[item.group]--;
        }

        var allHidden = true;

        // Work back through the groups hiding the underlines as required.
        for (i = (groupsToHide.length - 1); i > -1; i--) {
            if (groupsToHide[i] == 0) {
                AlterVisibilityOfAssociatedUnderline(groupUnderlineId[i], false);
            }
            else if (allHidden && i == (groupsToHide.length - 1)) {
                allHidden = false;
            }
            // If all the items have been hidden so far hide the last underline too.
            else if (allHidden) {
                allHidden = false;
                AlterVisibilityOfAssociatedUnderline(groupUnderlineId[i], false);
            }
        }
    }

    return contextMenuId;
}

function AlterVisibilityOfAssociatedUnderline(underLineId, visibility) {

    if (underLineId != null && underLineId != "") {

        var underlineElement = document.getElementById(underLineId);

        if (underlineElement != null) {

            if (visibility) {
                underlineElement.style.visibility = 'visible';
                underlineElement.style.display = 'block'
            }
            else {
                underlineElement.style.visibility = 'hidden';
                underlineElement.style.display = 'none'
            }
        }
    }
}

function ClearTimeouts() {
    if (_fadeTimeouts != null) {
        for (i = 0; i < _fadeTimeouts.length; i++) {
            clearTimeout(_fadeTimeouts[i]);
        }
    }
    _fadeTimeouts = [];
}

// This function chnages an elements opacity given its id.
function FadeOutElement(id, opacStart, opacEnd, millisec) {

    ClearTimeouts();
    //speed for each frame 
    var speed = Math.round(millisec / 100);
    var timer = 0;

    for (i = opacStart; i >= opacEnd; i--) {
        _fadeTimeouts.push(setTimeout("ChangeOpacityForElement(" + i + ",'" + id + "')", (timer * speed)));
        timer++;
    }
}

// This function changes the opacity of an elemnent given it's id.
// Works across browsers for different browsers
function ChangeOpacityForElement(opacity, id) {
    var object = document.getElementById(id).style;
    if (opacity != 0) {
        // **Cross browser compatibility code**
        object.opacity = (opacity / 100);
        object.MozOpacity = (opacity / 100);
        object.KhtmlOpacity = (opacity / 100);
        object.filter = "alpha(opacity=" + opacity + ")";
    }
    else {
        object.display = 'none';
    }
}

// This function is the click for the body of the document 
function ContextMouseDown() {

    if (_mouseOverContext) {
        return;
    }
    else {
        HideMenu()
    }
}

// This function fades out the context menu and then unselects the associated name control
function UnSelectedMenuItem() {
    if (_itemSelected) {

        FadeOutElement(_divContextMenu.id, 100, 0, 300);
        UnselectCurrentMenuItem();
    }
}

// Hides context menu without fading effect
function HideMenu()
{
    if (_itemSelected)
    {
        ChangeOpacityForElement(0, _divContextMenu.id);
        UnselectCurrentMenuItem();
    }
}

function UnselectCurrentMenuItem()
{
        _itemSelected = false;
        _currentContextMenuId = null;
        SwapStyle(_currentMenuItemId, 'msrs-MenuUIItemTableCell');
        _currentMenuItemId = null;
        ChangeReportItemStyle(_selectedItemId, "msrs-UnSelectedItem");
}

// This function walks back up the DOM tree until it finds the first occurrence
// of a given element. It then returns this element
function GetOuterElementOfType(element, type) {
    while (element.tagName.toLowerCase() != type) {

        element = element.parentNode;
    }
    return element;
}

// This function gets the corrdinates of the top left corner of a given element
function GetElementPosition(element) {
    element = GetOuterElementOfType(element, 'table');

    var left, top;
    left = top = 0;
    if (element.offsetParent) {
        do {
            left += element.offsetLeft;
            top += element.offsetTop;
        } while (element = element.offsetParent);
    }
    return { left: left, top: top };
}

function FocusContextMenuItem(menuItemId, focusStyle, blurStyle)
{
    SwapStyle(_currentMenuItemId, blurStyle);
    SwapStyle(menuItemId, focusStyle);
    document.getElementById(menuItemId).firstChild.focus();
    _currentMenuItemId = menuItemId;
}

// This function swaps the style using the id of a given element 
function SwapStyle(id, style) {
    if (document.getElementById) {
        var selectedElement = document.getElementById(id);
        if (selectedElement != null)
        {
            selectedElement.className = style;
        }
    }
}

// This function changes the style using the id of a given element
// and should only be called for catalog items in the tile or details view
function ChangeReportItemStyle(id, style) 
{
    if (!_itemSelected) 
    {
        if (document.getElementById) 
        {
            var selectedElement = document.getElementById(id);
            selectedElement.className = style;
            // Change the style on the end cell by drilling into the table.
            if (selectedElement.tagName.toLowerCase() == "table") 
            {
                var tbody = GetLastChildByTagName(selectedElement, "tbody");
                if (tbody != null) {
                    var tr = GetLastChildByTagName(tbody, "tr");
                    if (tr != null) {
                        var td = GetLastChildByTagName(tr, "td");
                        if (td != null) {
                            td.className = style + 'End';
                        }
                    }
                }
            }
        }
    }
}

function ChangeReportItemStyleOnFocus(id, currentStyle, unselectedStyle)
{
    _unselectedItemStyle = unselectedStyle;
    _tabFocusedItem = id;
    
    // We should unselect selected by mouse over item if there is one
    if(_mouseOverItem != '')
    {
        ChangeReportItemStyle(_mouseOverItem, _unselectedItemStyle);
        _mouseOverItem = '';
    }
    
    ChangeReportItemStyle(id, currentStyle);
}

function ChangeReportItemStyleOnBlur(id, style)
{    
    ChangeReportItemStyle(id, style);
    _tabFocusedItem = '';
}

function ChangeReportItemStyleOnMouseOver(id, currentStyle, unselectedStyle)
{
    _unselectedItemStyle = unselectedStyle;
    _mouseOverItem = id;
    
    // We should unselect tabbed item if there is one
    if(_tabFocusedItem != '')
    {
        ChangeReportItemStyle(_tabFocusedItem, _unselectedItemStyle);
        _tabFocusedItem = '';
    }
    
    ChangeReportItemStyle(id, currentStyle);
}

function ChangeReportItemStyleOnMouseOut(id, style)
{    
    ChangeReportItemStyle(id, style);
    _mouseOverItem = '';
}

// This function is used to set the style of the search bar on the onclick event.
function SearchBarClicked(id, defaultText, style) {
    var selectedElement = document.getElementById(id);
    if (selectedElement.value == defaultText) {
        selectedElement.value = "";
        selectedElement.className = style;
    }
}

// This function is used to set the style of the search bar on the onblur event.
function SearchBarBlured(id, defaultText, style) {
    var selectedElement = document.getElementById(id);
    if (selectedElement.value == "") {
        selectedElement.value = defaultText;
        selectedElement.className = style;
    }
}

function ResetSearchBar(searchTextBoxID,defaultSearchValue) {

    var selectedElement = document.getElementById(searchTextBoxID);
    if (selectedElement != null) {
        if (selectedElement.value == defaultSearchValue) {
            selectedElement.className = 'msrs-searchDefaultFont';
        }
        else {
            selectedElement.className = 'msrs-searchBarNoBorder';
        }
    }
}

function OnLink() 
{
    _onLink = true;     
}

function OffLink() 
{
    _onLink = false;
}

function ShouldDelete(confirmMessage) {
    if (_selectedIdHiddenField.value != null || _selectedIdHiddenField.value != "") {
        var message = confirmMessage.replace("{0}", _selectedIdHiddenField.value);
        var result = confirm(message);
        if (result == true) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return false;
    }
}

function UpdateValidationButtonState(promptCredsRdoBtnId, typesDropDownId, forbiddenTypesConfigString, validateButtonId)
{
    var dropdown = document.getElementById(typesDropDownId);
    
    if(dropdown == null)
    {
        return;
    }
    
    var selectedValue = dropdown.options[dropdown.selectedIndex].value;    
    var forbiddenTypes = forbiddenTypesConfigString.split(":");
    
    var chosenForbiddenType = false;
    
    for (i = 0; i < forbiddenTypes.length; i++)
    {
        if(forbiddenTypes[i] == selectedValue) 
        {
            chosenForbiddenType = true;
        }   
    }
    
    var isDisabled = chosenForbiddenType || IsRadioButtonChecked(promptCredsRdoBtnId);
    
    ChangeDisabledButtonState(validateButtonId, isDisabled);
}

function ChangeDisabledButtonState(buttonId, isDisabled)
{
    var button = document.getElementById(buttonId);
    
    if(button != null)
    {
        button.disabled = isDisabled;
    }
}

function IsRadioButtonChecked(radioButtonId)
{
    var rbtn = document.getElementById(radioButtonId);
    
    if(rbtn != null && rbtn.checked)
    {
        return true;
    }
    
    return false;
}

function GetLastChildByTagName(element, tagName)
{
    var element_children = element.getElementsByTagName(tagName);
    if (element_children && element_children.length > 0) {
        return element_children[element_children.length - 1];
    }

    return null;
}
