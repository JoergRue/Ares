var elementsSource;

$(document).on('pageshow', '#elements', function () {
    elementsSource = new EventSource('/event-stream?channel=Elements&t=' + new Date().getTime());
    elementsSource.addEventListener('error', function (e) {
        console.log(e);
    }, false);

    $.ss.eventReceivers = { "document": document };
    $(elementsSource).handleServerEvents({
        handlers: {
            NewProjectInfo: function (msg, e) {
                var element = getElementById("loadedProject_Elements");
                if (element) element.innerHTML = msg.Name;
            },
            ModeInfo: function (msg, e) {
                var activeModeElement = getElementById("activeMode_Elements");
                if (activeModeElement) activeModeElement.innerHTML = msg.Title;
                var elementsSelection = getElementById("modeElementsSelection");
                elementsSelection.innerHTML = "";
                for (i = 0; i < msg.Triggers.length; ++i) {
                    var trigger = msg.Triggers[i];
                    var listElement = document.createElement('li');
                    listElement.setAttribute("class", "trigger");
                    var newTriggerIndicator = document.createElement('img');
                    newTriggerIndicator.setAttribute("id", "indicator" + trigger.Id);
                    newTriggerIndicator.setAttribute("class", "modeElementTriggerIndicator");
                    newTriggerIndicator.src = trigger.IsActive ? "Images/greenlight.png" : "Images/redlight.png";
                    listElement.appendChild(newTriggerIndicator);
                    var newTriggerButton = document.createElement('input');
                    newTriggerButton.type = "button";
                    newTriggerButton.onclick = function () { triggerModeElement(this.id); };
                    newTriggerButton.setAttribute("id", trigger.Id);
                    newTriggerButton.setAttribute("value", trigger.Name);
                    newTriggerButton.setAttribute("class", "elementTriggerButton");
                    listElement.appendChild(newTriggerButton);
                    elementsSelection.appendChild(listElement);
                }
                setColumnWidth(elementsSelection);
            },
            ActiveElementsInfo: function (msg, e) {
                var elementIndicatorList = document.getElementsByClassName("modeElementTriggerIndicator");
                for (i = 0; i < elementIndicatorList.length; ++i) {
                    elementIndicatorList[i].src = "Images/redlight.png";
                    elementIndicatorList[i].alt = "off";
                }
                var activeList = getElementById("activeModeElements");
                var activeListText = "";
                for (i = 0; i < msg.Triggers.length; ++i) {
                    updateModeElementIndicator(msg.Triggers[i].Id, msg.Triggers[i].IsActive);
                    activeListText += msg.Triggers[i].Name;
                    if (i < msg.Triggers.length - 1)
                        activeListText += ", ";
                }
                if (activeList != null)
                    activeList.innerHTML = activeListText;
            },
        },
        receivers: {

        },
        success: function (selector, msg, json) {
        },
    });

});

$(document).on('pagebeforehide', '#elements', function () {
    elementsSource.close();
});


function triggerModeElement(id) {
    $.getJSON("triggerElement?Id=" + id, function (resp) { });
}

function updateModeElementIndicator(id, isActive) {
    var indicator = getElementById("indicator" + id);
    if (indicator) {
        indicator.src = isActive ? "Images/greenlight.png" : "Images/redlight.png";
        indicator.alt = isActive ? "on" : "off";
    }
}
