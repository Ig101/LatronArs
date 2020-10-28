DOMGetBoundingClientRect = (element, parm) => { return element.getBoundingClientRect(); };

browserResize = {
    registerResizeCallback: function () {
        window.addEventListener("resize", browserResize.resized);
    },
    resized: function () {
        DotNet.invokeMethodAsync("LatronArs.WebClient", 'PushResize').then(data => data);
    }
}