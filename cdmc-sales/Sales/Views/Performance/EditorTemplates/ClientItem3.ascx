<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%= Html.Telerik().DropDownList()
        .DropDownHtmlAttributes(new { style = string.Format("width:{0}px", 200) })
        .Name("Item3Score")
        .BindTo(new SelectList((IEnumerable)ViewData["Item3s"], "ID", "Name"))
%>