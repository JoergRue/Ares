var plsource;

$(document).on('pageshow', '#playlist', function () {

    plsource = new EventSource('/event-stream?channel=Playlist&t=' + new Date().getTime());
    plsource.addEventListener('error', function (e) {
        console.log(e);
    }, false);

    $.ss.eventReceivers = { "document": document };
    $(plsource).handleServerEvents({
        handlers: {
            ErrorInfo: function (msg, e) {
                toast(msg.ErrorMessage);
            },
            NewProjectInfo: function (msg, e) {
                var element = getElementById("loadedProject_PL");
                if (element) element.innerHTML = msg.Name;
            },
            MusicInfo: function (msg, e) {
                var element = getElementById("currentMusic_PL");
                if (element) element.innerHTML = msg.LongTitle;
            },
            MusicRepeatInfo: function (msg, e) {
                var repeatButton = getElementById("repeatButton_PL");
                if (repeatButton == null) return;
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
            MusicListInfo: function (msg, e) {
                var musicList = getElementById("playlistList_PL");
                if (!musicList) return;
                musicList.innerHTML = "";
                for (i = 0; i < msg.Ids.length; ++i) {
                    var newLink = document.createElement('a');
                    newLink.setAttribute("class", "musicListLink");
                    newLink.innerHTML = msg.Titles[i];
                    newLink.id = msg.Ids[i];
                    newLink.onclick = function () { selectElement_PL(this.id); }
                    var newItem = document.createElement('li');
                    newItem.className = "playlist";
                    newItem.appendChild(newLink);
                    musicList.appendChild(newItem);
                }
            },
        },
        receivers: {

        },
        success: function (selector, msg, json) {
        },
    });
});

$(document).on('pagebeforehide', '#playlist', function () {
   plsource.close();
});

function selectElement_PL(id) {
    $.getJSON("selectMusicElement?Id=" + id, function (resp) { });
}

function stopAll_PL() {
    $.getJSON("mainControl?Command=Stop", function (resp) { });
}

function back_PL() {
    $.getJSON("mainControl?Command=Back", function (resp) { });
}

function forward_PL() {
    $.getJSON("mainControl?Command=Forward", function (resp) { });
}
function repeatOn_PL() {
    $.getJSON("mainControl?Command=RepeatOn", function (resp) { });
}
function repeatOff_PL() {
    $.getJSON("mainControl?Command=RepeatOff", function (resp) { });
}
