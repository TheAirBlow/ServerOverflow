﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ServerOverflow.Database;
using ServerOverflow.Models;
using static System.Int32;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace ServerOverflow.Controllers;

/// <summary>
/// Database search controller
/// </summary>
[Route("search")]
public class SearchController : Controller {
    [Route("servers")]
    public async Task<IActionResult> Servers() {
        var account = await HttpContext.GetAccount();
        if (account == null) return Redirect("/user/login");
        if (!account.HasPermission(Permission.SearchServers))
            return Unauthorized();
        
        var model = new SearchModel();
        if (!Request.Query.TryGetValue("q", out var query)) 
            return View(model);
        model.Query = query;
        
        model.CurrentPage = 1;
        if (Request.Query.TryGetValue("page", out var pageStr))
            if (TryParse(pageStr, out var page))
                model.CurrentPage = page;
        if (model.CurrentPage < 1)
            model.CurrentPage = 1;
        
        try {
            var doc = Query.Servers(model.Query!);
            var find = Database.Controller.Servers.Find(doc);
            model.TotalMatches = await find.CountDocumentsAsync();
            if (model.TotalMatches == 0) {
                model.Message = "No matches found for your query";
                model.Success = false;
                return View(model);
            }
            
            model.TotalPages = (int)Math.Ceiling(model.TotalMatches / 50f);
            if (model.CurrentPage > model.TotalPages) model.CurrentPage = model.TotalPages;
            using var cursor = await find.Skip(50 * (model.CurrentPage-1)).Limit(50).ToCursorAsync();
            model.Items = []; model.Items.AddRange(cursor.ToList());
        } catch (Exception e) {
            model.Message = e.Message;
            model.Success = false;
        }
        
        return View(model);
    }
    
    [Route("accounts")]
    public async Task<IActionResult> Accounts() {
        var account = await HttpContext.GetAccount();
        if (account == null) return Redirect("/user/login");
        if (!account.HasPermission(Permission.SearchAccounts))
            return Unauthorized();
        return View();
    }
}
