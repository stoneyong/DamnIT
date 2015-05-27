<%@ WebHandler Language="C#" Class="ajax" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Newtonsoft.Json.Linq;
using Service;
using Newtonsoft.Json;

public class ajax : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "application/json";
        switch (context.Request["action"])
        {
            case "loaduser":
                LoadUser(context);
                break;
            case "searchuser":
                SearchUser(context);
                break;
            case "loadscore":
                LoadScore(context);
                break;
            case "markuser":
                MarkUser(context);
                break;
            case "getInitData":
                GetInitData();
                break;
        }
    }


    public void GetInitData() { 
        
     var  con=ConsteService.GetViewModelByUserId("4c485619-e958-4c0b-b7fa-3e128721898e");
    }
    
    void LoadUser(HttpContext context)
    {
        string uid = context.Request["uid"];
        if (string.IsNullOrEmpty(uid))
        {
            context.Response.Write(JsonConvert.SerializeObject(UserService.GetUser()));
        }
        else
        {
            context.Response.Write(JsonConvert.SerializeObject(UserService.GetUserByID(uid)));
        }
        
    }

    void SearchUser(HttpContext context)
    {
        var strResult = UserService.GetUserList(context.Request["term"]);
        var users = (JsonConvert.DeserializeObject(strResult) as JObject)["users"] as JArray;
        context.Response.Write(
            JsonConvert.SerializeObject(
                users.Where(jUser => jUser != null)
                    .Select(jUser => new {label = jUser["name"], value = jUser["id"]})));
    }

    void LoadScore(HttpContext context)
    {
        string uid = context.Request["uid"];
        var score = UserService.GetUserMsg(string.IsNullOrEmpty(uid) ? UserService.GetUser().UserID.ToString() : uid, DateTime.Parse(context.Request["sdate"]),
            DateTime.Parse(context.Request["edate"]));
        context.Response.Write(JsonConvert.SerializeObject(score));
    }

    private void MarkUser(HttpContext context)
    {
        var uid = context.Request["uid"];
        var isSelf = string.IsNullOrEmpty(uid);
        if (isSelf) uid = UserService.GetUser().UserID.ToString();
        var workload = decimal.Parse(context.Request["workload"]);
        var efficiency = decimal.Parse(context.Request["efficiency"]);
        var cooperative = decimal.Parse(context.Request["cooperative"]);
        var resultList = new List<int>();
        resultList.Add(AppraiseService.AppraiseOthers(uid, isSelf ? 1 : 2, 1, workload));
        resultList.Add(AppraiseService.AppraiseOthers(uid, isSelf ? 1 : 2, 2, efficiency));
        resultList.Add(AppraiseService.AppraiseOthers(uid, isSelf ? 1 : 2, 3, cooperative));
        context.Response.Write(JsonConvert.SerializeObject(resultList));
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}