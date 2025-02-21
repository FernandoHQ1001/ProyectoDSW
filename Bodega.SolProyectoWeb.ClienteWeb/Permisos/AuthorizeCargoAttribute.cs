using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bodega.SolProyectoWeb.Entidades.Core;

public class AuthorizeCargoAttribute : AuthorizeAttribute
{
    private readonly string _requiredCargo;

    public AuthorizeCargoAttribute(string requiredCargo)
    {
        _requiredCargo = requiredCargo;
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        var usuario = httpContext.Session["Usuario"] as Usuario;
        return usuario != null && usuario.cargo == _requiredCargo;
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        HttpContext.Current.Session["Usuario"] = null;
        filterContext.Result = new RedirectResult("~/Login/Login?mensaje=no-autorizado");

    }
}