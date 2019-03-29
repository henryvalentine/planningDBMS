//Draggable Div by surren @ http://www.diffusedreality.com
var x;
var y;
var element;
var being_dragged = false;
function mouser(event)
{
    if (event.offsetX || event.offsetY)
    {
        x=event.offsetX;
        y=event.offsetY;
    }
    else {
        x=event.pageX;
        y=event.pageY;
    }
    document.getElementById('X').innerHTML = x +'px';
    document.getElementById('Y').innerHTML = y +'px';
    document.getElementById('X-coord').innerHTML = x +'px';
    document.getElementById('Y-coord').innerHTML = y +'px';
    if (being_dragged == true)
    {
        document.getElementById(element).style.left = x +'px';
        document.getElementById(element).style.top = y +'px';
    }
}

function mouse_down(ele_name)
{
    being_dragged = true;
    element = ele_name;
    document.getElementById(element).style.cursor = 'move';
}

function mouse_up()
{
    being_dragged = false;
    document.getElementById(element).style.top = y +'px';
    document.getElementById(element).style.left = x +'px';
    document.getElementById(element).style.cursor = 'auto';
}