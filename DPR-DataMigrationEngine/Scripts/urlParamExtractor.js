function getURLParameter(sParam)
{
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    var returnParam = '';
    for(var i = 0; i < sURLVariables.length; i++)
    {
        var sParameterName = sURLVariables[i].split('=');
        if(sParameterName[0] == sParam)
        {
            returnParam = sParameterName[1];
            break;
        }
    }
    return returnParam;
}