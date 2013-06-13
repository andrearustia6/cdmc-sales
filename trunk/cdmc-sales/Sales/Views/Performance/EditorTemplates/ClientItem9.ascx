<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%= Html.Telerik().DropDownList()
    .DropDownHtmlAttributes(new { style = string.Format("width:{0}px", 200) })
            .Name("Item9Score")
        .BindTo(new SelectList((IEnumerable)ViewData["Item9s"], "ID", "Name"))
%>