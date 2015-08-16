var source;

$(document).on('pageshow', '#control', function () {

    source = new EventSource('/event-stream?channel=Control&t=' + new Date().getTime());
    source.addEventListener('error', function (e) {
        console.log(e);
    }, false);

    $.ss.eventReceivers = { "document": document };
    $(source).handleServerEvents({
        handlers: {
            ErrorInfo: function(msg, e) {
                toast(msg.ErrorMessage);
            },
            NewProjectInfo: function (msg, e) {
                var element = getElementById("loadedProject");
                if (!element) return;
                element.innerHTML = msg.Name;
                var modeSelection = getElementById("modeSelection2");
                if (!modeSelection) return;
                modeSelection.innerHTML = "";
                for (i = 0; i < msg.Modes.length; ++i) {
                    var mode = msg.Modes[i];
                    var newModeButton = document.createElement('input');
                    newModeButton.type = "button";
                    newModeButton.onclick = function () { selectMode2(this.value); };
                    newModeButton.setAttribute("value", mode);
                    newModeButton.setAttribute("class", "modeButton");
                    var listElement = document.createElement('li');
                    listElement.setAttribute("class", "modeTrigger");
                    listElement.appendChild(newModeButton);
                    modeSelection.appendChild(listElement);
                }
                setColumnWidth(modeSelection);
            },
            VolumeInfo: function (msg, e) {
                switch (msg.Id) {
                    case 0: setVolumeValue("soundsValueSlider", msg.Value); break;
                    case 1: setVolumeValue("musicValueSlider", msg.Value); break;
                    case 2: setVolumeValue("overallValueSlider", msg.Value); break;
                }
            },
            ModeInfo: function (msg, e) {
                var activeModeElement = getElementById("activeMode2");
                if (activeModeElement) activeModeElement.innerHTML = msg.Title;
                var elementsSelection = getElementById("elementsSelection2");
                if (!elementsSelection) return;
                elementsSelection.innerHTML = "";
                for (i = 0; i < msg.Triggers.length; ++i) {
                    var trigger = msg.Triggers[i];
                    var listElement = document.createElement('li');
                    listElement.setAttribute("class", "trigger");
                    var spanElement = document.createElement('span');
                    var newTriggerIndicator = document.createElement('img');
                    newTriggerIndicator.setAttribute("id", "indicator" + trigger.Id);
                    newTriggerIndicator.setAttribute("class", "elementTriggerIndicator");
                    newTriggerIndicator.src = trigger.IsActive ? "Images/greenlight.png" : "Images/redlight.png";
                    spanElement.appendChild(newTriggerIndicator);
                    var newTriggerButton = document.createElement('input');
                    newTriggerButton.type = "button";
                    newTriggerButton.onclick = function () { triggerElement(this.id); };
                    newTriggerButton.setAttribute("id", trigger.Id);
                    newTriggerButton.setAttribute("value", trigger.Name);
                    newTriggerButton.setAttribute("class", "elementTriggerButton");
                    spanElement.appendChild(newTriggerButton);
                    listElement.appendChild(spanElement);
                    elementsSelection.appendChild(listElement);
                }
                setColumnWidth(elementsSelection);
            },
            ActiveElementsInfo: function (msg, e) {
                var elementIndicatorList = document.getElementsByClassName("elementTriggerIndicator");
                for (i = 0; i < elementIndicatorList.length; ++i) {
                    elementIndicatorList[i].src = "Images/redlight.png";
                    elementIndicatorList[i].alt = "off";
                }
                var activeList = getElementById("activeElements2");
                if (activeList != null) {
                    var activeListText = "";
                    for (i = 0; i < msg.Triggers.length; ++i) {
                        updateElementIndicator(msg.Triggers[i].Id, msg.Triggers[i].IsActive);
                        activeListText += msg.Triggers[i].Name;
                        if (i < msg.Triggers.length - 1)
                            activeListText += ", ";
                    }
                    activeList.innerHTML = activeListText;
                }
                setAnimation("activeElements2", "elementsParent", 1);
            },
            MusicInfo: function (msg, e) {
                var currentMusic = getElementById("currentMusic");
                if (currentMusic != null) currentMusic.innerHTML = msg.LongTitle;
                setAnimation("currentMusic", "musicParent", 2);
            },
            MusicListInfo: function (msg, e) {
                var musicList = getElementById("playlistList");
                if (!musicList) return;
                musicList.innerHTML = "";
                for (i = 0; i < msg.Ids.length; ++i) {
                    var newLink = document.createElement('a');
                    newLink.setAttribute("class", "musicListLink");
                    newLink.innerHTML = msg.Titles[i];
                    newLink.id = msg.Ids[i];
                    newLink.onclick = function () { selectElement(this.id); }
                    var newItem = document.createElement('li');
                    newItem.className = "playlist";
                    newItem.appendChild(newLink);
                    musicList.appendChild(newItem);
                }
            },
            MusicRepeatInfo: function (msg, e) {
                var repeatButton = getElementById("repeatButton");
                if (repeatButton == null)
                    return;
                if (msg.Repeat) {
                    repeatButton.class = "repeatButtonChecked";
                    repeatButton.value = "Repeat (on)";
                    repeatButton.onclick = function () { repeatOff(); }
                    repeatButton.onmouseup = function () { switchImg(this, "Images/repeat_on.png"); }
                    repeatButton.src = "Images/repeat_on.png";
                }
                else {
                    repeatButton.class = "repeatButtonUnchecked";
                    repeatButton.setAttribute("alt", "Repeat (off)");
                    repeatButton.onclick = function () { repeatOn(); }
                    repeatButton.onmouseup = function () { switchImg(this, "Images/repeat_off.png"); }
                    repeatButton.src = "Images/repeat_off.png";
                }
            },
        },
        receivers: {

        },
        success: function (selector, msg, json) {
        },
    });

    setColumnWidth(getElementById("specialModeTriggers"));
});

$(document).on('pagebeforehide', '#control', function () {
    source.close();
});

function changeVolume(volId, volValue) {
    $.getJSON("changeVolume?Id=" + volId + "&Value=" + volValue, function (resp) { });
}

function selectMode2(mode) {
    $.getJSON("selectMode?Title=" + mode, function (resp) { });
    switchToElements();
}

function triggerElement(id) {
    $.getJSON("triggerElement?Id=" + id, function (resp) { });
}

function updateElementIndicator(id, isActive) {
    var indicator = getElementById("indicator" + id);
    if (indicator) {
        indicator.src = isActive ? "Images/greenlight.png" : "Images/redlight.png";
        indicator.alt = isActive ? "on" : "off";
    }
}

function setVolumeValue(elementId, value)
{
    var element = getElementById(elementId);
    if (element) {
        element.value = value;
    }
}

function stopAll() {
    $.getJSON("mainControl?Command=Stop", function (resp) { });
}

function back() {
    $.getJSON("mainControl?Command=Back", function (resp) { });
}

function forward() {
    $.getJSON("mainControl?Command=Forward", function (resp) { });
}
function repeatOn() {
    $.getJSON("mainControl?Command=RepeatOn", function (resp) { });
}
function repeatOff() {
    $.getJSON("mainControl?Command=RepeatOff", function (resp) { });
}

function getStyle(elementId) {
    var styleObj;
    var el = getElementById(elementId);

    if (typeof window.getComputedStyle != "undefined") {
        styleObj = window.getComputedStyle(el, null);
    } else if (el.currentStyle != "undefined") {
        styleObj = el.currentStyle;
    }
    return styleObj;
}

function switchToPlaylist() {
    if (getStyle("inlinePlaylist").display == 'block') {
        return;
    }
    if (getStyle("elements").display == 'block') {
        getElementById("inlinePlaylist").style.display = 'block';
        getElementById("elements").style.display = 'none';
        $.getJSON("resendInfo?InfoId=MusicList", function (resp) { });
    }
    else {
        $.mobile.pageContainer.pagecontainer("change", "/Playlist", {});
    }
}

function switchToElements() {
    if (getStyle("elements").display == 'block') {
        return;
    }
    if (getStyle("inlinePlaylist").display == 'block') {
        getElementById("inlinePlaylist").style.display = 'none';
        getElementById("elements").style.display = 'block';
        $.getJSON("resendInfo?InfoId=Elements", function (resp) { });
    }
    else {
        $.mobile.pageContainer.pagecontainer("change", "/Elements", {});
    }
}

function switchToTags() {
    $.mobile.pageContainer.pagecontainer("change", "/Tags", { });
}

function findKeyframesRule(rule) {
    var ss = document.styleSheets;
    for (var i = 0; i < ss.length; ++i) if (ss[i].cssRules) {
        for (var j = 0; j < ss[i].cssRules.length; ++j) {
            if (ss[i].cssRules[j].name == rule) {
                return ss[i].cssRules[j];
            }
        }
    }
    return null;
}

function setAnimation(element, parent, ruleIndex) {
    var e = getElementById(element);
    if (e == null) return;
    var p = getElementById(parent);
    var w = e.offsetWidth;
    var w2 = p.offsetWidth;
    if (w > w2)
    {
        var w3 = w - w2 + 20;
        removeClass(e, "noanimation");
        addClass(e, "animation");
        addClass(e, "animation" + ruleIndex)
        var rule = findKeyframesRule("bouncing-text" + ruleIndex);
        if (rule) {
            rule.deleteRule("1");
            rule.appendRule("100% { transform: translateX(-" + w3 + "px); -mox-transform: translateX(- " + w3 + "px); -webkit-transform: translateX(-" + w3 + "px); }");
        }
        var rule2 = findKeyframesRule("bouncing-text" + ruleIndex + "-webkit");
        if (rule2) {
            rule2.deleteRule("1");
            rule2.appendRule("100% { -webkit-transform: translateX(-" + w3 + "px); }");
        }
        var rule3 = findKeyframesRule("bouncing-text" + ruleIndex + "-moz");
        if (rule3) {
            rule3.deleteRule("1");
            rule3.appendRule("100% { -moz-transform: translateX(-" + w3 + "px); }");
        }
    }
    else
    {
        removeClass(e, "animation");
        addClass(e, "noanimation");
    }
}

function selectElement(id) {
    $.getJSON("selectMusicElement?Id=" + id, function (resp) { });
}

