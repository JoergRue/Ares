function openProject_Projects(fileName)
{
    $.getJSON("openProject?ProjectFileName=" + fileName, function (resp) { });
    $.mobile.pageContainer.pagecontainer("change", "/Control", {});
}
