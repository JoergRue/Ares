$(document).on("pagecontainershow", function (event, ui) {
    var pageId = $('body').pagecontainer('getActivePage').prop('id');
    if (pageId == "tags") {
        $.getJSON("resendInfo?InfoId=Tags", function (resp) { });
    }
    else if (pageId == "elements") {
        $.getJSON("resendInfo?InfoId=Elements", function (resp) { });
    }
    else if (pageId == "modes") {
        $.getJSON("resendInfo?InfoId=Modes", function (resp) { });
    }
    else if (pageId == "playlist") {
        $.getJSON("resendInfo?InfoId=MusicList", function (resp) { });
    }
});

function getElementById(id)
{
    var page = $('body').pagecontainer('getActivePage');
    var element = $('#' + id, page);
    if (element) {
        var domElement = element[0];
        return domElement;
    }
    else return null;
}

function toPlaylist() {
    $.mobile.pageContainer.pagecontainer("change", "/Playlist", {});
}

function innerWidth(el) {
    var kids = el.children;
    var w = 0;
    for (var i = 0; i < kids.length; i++)
        w += kids[i].offsetWidth;
    return w;
}

function setColumnWidth(list) {
    var items = list.children;
    var mW = 0;
    for (var i = 0; i < items.length; i++) {
        var w = innerWidth(items[i]);
        if (w > mW)
            mW = w;
    }
    mW += 10;
    list.setAttribute("style",
                      "column-width:" + mW + "px;" +
                      "-moz-column-width:" + mW + "px;" +
                      "-webkit-column-width:" + mW + "px");
}

function getProjects() {
    $.mobile.pageContainer.pagecontainer("change", "/GetProjects", {});
}

function switchImg(elemnt, path) {
    elemnt.src = path;
}

function removeClass(element, aClassName) {
    element.className = element.className.replace(new RegExp('(?:^|\\s)' + aClassName + '(?!\\S)'), '');
}

function addClass(element, aClassName) {
    element.className = element.className + " " + aClassName;
}

var toast = function (msg) {
    $("<div class='ui-loader ui-overlay-shadow ui-body-e ui-corner-all'><h3>" + msg + "</h3></div>")
	.css({
	    display: "block",
	    opacity: 0.90,
	    position: "fixed",
	    padding: "7px",
	    "text-align": "center",
	    width: "270px",
	    left: ($(window).width() - 284) / 2,
	    top: $(window).height() / 2
	})
	.appendTo($.mobile.pageContainer).delay(1500)
	.fadeOut(400, function () {
	    $(this).remove();
	});
}

function showSettings(pageLink) {
    $.mobile.pageContainer.pagecontainer("change", "/Settings?SourcePage=" + pageLink, {});
}

