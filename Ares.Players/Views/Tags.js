var tagsSource;

$(document).on('pageshow', '#tags', function () {

    tagsSource = new EventSource('/event-stream?channel=Tags&t=' + new Date().getTime());
    tagsSource.addEventListener('error', function (e) {
        console.log(e);
    }, false);

    $.ss.eventReceivers = { "document": document };
    $(tagsSource).handleServerEvents({
        handlers: {
            TagInfo: function (msg, e) {
                console.log("Received TagInfo");
                var categorySelect = getElementById('categorySelect');
                if (!categorySelect) return;
                categorySelect.innerHTML = "";
                for (i = 0; i < msg.Categories.length; ++i) {
                    var optionElement = document.createElement("option");
                    optionElement.selected = (msg.Categories[i].Id == msg.ActiveCategory);
                    optionElement.id = msg.Categories[i].Id;
                    optionElement.text = msg.Categories[i].Name;
                    categorySelect.appendChild(optionElement);
                }
                if (msg.Categories.length > 0) {
                    updateTags(msg.TagsPerCategory[msg.ActiveCategory]);
                }
            },
            ActiveTagInfo: function (msg, e) {
                var fadeTimeInput = getElementById('fadeTimeInput');
                if (!fadeTimeInput) return;
                fadeTimeInput.value = msg.FadeTime;
                var combineCombo = getElementById('combineSelect');
                if (!combineCombo) return;
                combineCombo.selectedIndex = msg.CategoryCombination;
                var elementIndicatorList = document.getElementsByClassName("elementTriggerIndicator");
                for (i = 0; i < elementIndicatorList.length; ++i) {
                    elementIndicatorList[i].src = "Images/redlight.png";
                    elementIndicatorList[i].alt = "off";
                }
                var activeList = getElementById("activeTags");
                var activeListText = "";
                for (i = 0; i < msg.Tags.length; ++i) {
                    updateTagElementIndicator(msg.Tags[i].Id, msg.Tags[i].IsActive);
                    activeListText += msg.Tags[i].Name;
                    if (i < msg.Tags.length - 1)
                        activeListText += ", ";
                }
                activeList.innerHTML = activeListText;
            },
            TagFadingInfo: function (msg, e) {
                var fadeTimeInput = getElementById('fadeTimeInput');
                if (!fadeTimeInput) return;
                fadeTimeInput.value = msg.FadeTime;
                var fadeBox = getElementById('fadeOnChangeBox');
                if (!fadeBox) return;
                fadeBox.checked = msg.FadeOnlyOnChange;
            }
        },
        receivers: {

        },
        success: function (selector, msg, json) {
        },
    });


});

$(document).on('pagebeforehide', '#tags', function () {
    tagsSource.close();
});

function updateTags(tagList) {
    var elementsSelection = getElementById("elementsSelection");
    if (!elementsSelection) return;
    elementsSelection.innerHTML = "";
    for (i = 0; i < tagList.length; ++i) {
        var trigger = tagList[i];
        var listElement = document.createElement('li');
        listElement.setAttribute("class", "trigger");
        var newTriggerIndicator = document.createElement('img');
        newTriggerIndicator.setAttribute("id", "indicator" + trigger.Id);
        newTriggerIndicator.setAttribute("class", "elementTriggerIndicator");
        newTriggerIndicator.src = trigger.IsActive ? "Images/greenlight.png" : "Images/redlight.png";
        listElement.appendChild(newTriggerIndicator);
        var newTriggerButton = document.createElement('input');
        newTriggerButton.type = "button";
        newTriggerButton.onclick = function () { triggerTagElement(this.id); };
        newTriggerButton.setAttribute("id", trigger.Id);
        newTriggerButton.setAttribute("value", trigger.Name);
        newTriggerButton.setAttribute("class", "elementTriggerButton");
        listElement.appendChild(newTriggerButton);
        elementsSelection.appendChild(listElement);
    }
    setColumnWidth(elementsSelection);
}

function triggerTagElement(id) {
    $.getJSON("switchTag?Id=" + id);
}

function updateTagElementIndicator(id, isActive) {
    var indicator = getElementById("indicator" + id);
    if (indicator) {
        indicator.src = isActive ? "Images/greenlight.png" : "Images/redlight.png";
        indicator.alt = isActive ? "on" : "off";
    }
}

function SelectCategory() {
    var categorySelect = getElementById('categorySelect');
    var selectedId = categorySelect.options[categorySelect.selectedIndex].id;
    $.getJSON("selectTagCategory?Id=" + selectedId, function (resp) { });
}

function ChangeTagCombine() {
    var combineSelect = getElementById('combineSelect');
    var selectedOption = combineSelect.selectedIndex;
    $.getJSON("selectTagCombination?Option=" + selectedOption, function (resp) { });
}

function SetFading() {
    var timeInput = getElementById('fadeTimeInput');
    var fadeOnChangeBox = getElementById('fadeOnChangeBox');
    $.getJSON("setTagFading?Time=" + timeInput.value + "&OnlyOnChange=" + fadeOnChangeBox.checked);
}

function RemoveAllTags() {
    $.getJSON("removeAllTags");
}
