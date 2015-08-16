var modeSource;

$(document).on('pageshow', '#modes', function () {

    modeSource = new EventSource('/event-stream?channel=Modes&t=' + new Date().getTime());
    modeSource.addEventListener('error', function (e) {
        console.log(e);
    }, false);

    $.ss.eventReceivers = { "document": document };
    $(modeSource).handleServerEvents({
        handlers: {
            NewProjectInfo: function (msg, e) {
                var element = getElementById("loadedProject_Modes");
                if (element == null) return;
                element.innerHTML = msg.Name;
                var modeSelection = getElementById("modeSelection_Modes");
                if (!modeSelection) return;
                modeSelection.innerHTML = "";
                for (i = 0; i < msg.Modes.length; ++i) {
                    var mode = msg.Modes[i];
                    var newModeButton = document.createElement('input');
                    newModeButton.type = "button";
                    newModeButton.onclick = function () { selectMode_Modes(this.value); };
                    newModeButton.setAttribute("value", mode);
                    newModeButton.setAttribute("class", "modeButton");
                    var listElement = document.createElement('li');
                    listElement.setAttribute("class", "modeTrigger");
                    listElement.appendChild(newModeButton);
                    modeSelection.appendChild(listElement);
                }
                setColumnWidth(modeSelection);
            },
            ModeInfo: function (msg, e) {
                var element = getElementById("activeMode_Modes");
                if (element) element.innerHTML = msg.Title;
            },
        },
        receivers: {

        },
        success: function (selector, msg, json) {
        },
    });
});

$(document).on('pagebeforehide', '#modes', function () {
    modeSource.close();
});



function selectMode_Modes(mode) {
    $.getJSON("selectMode?Title=" + mode, function (resp) { });
    $.mobile.pageContainer.pagecontainer("change", "/Elements", {});
}
