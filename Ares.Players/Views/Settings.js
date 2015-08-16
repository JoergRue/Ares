function cancel_Settings() {
    $.mobile.back();
}

function submitData_Settings(sourcePage) {
    var lang = getElementById("languageDe").checked ? "de" : "en";
    var fadeTime = getElementById("fadeTimeInput").value;
    var fadeOption = 0;
    if (getElementById("manualFadingOutIn").checked) fadeOption = 1;
    else if (getElementById("manualFadingCross").checked) fadeOption = 2;
    var onAllSpeakers = getElementById("allSpeakers").checked;
    $.getJSON("changeSettings?PlayMusicOnAllSpeakers=" + onAllSpeakers + "&FadingOption=" + fadeOption + "&FadingTime=" + fadeTime + "&Language=" + lang);
    $.mobile.pageContainer.pagecontainer("change", "/" + sourcePage, { reload: 'true' });
}