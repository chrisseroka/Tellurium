﻿var baseReplaceWith = $.fn.replaceWith;
$.fn.replaceWith = function () {
    var element;
    if (typeof arguments[0] == "string") {
        element = arguments[0] = $(arguments[0]);
    }
    var res = baseReplaceWith.apply(this, arguments);
    if (element != null) {
        $.ui.modernUnobtrusive.parse(element);
    }
    return res;
};