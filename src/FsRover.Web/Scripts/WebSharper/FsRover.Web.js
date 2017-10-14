(function()
{
 "use strict";
 var Global,FsRover,Web,Client,WebSharper,UI,Next,Doc,AttrProxy,Remoting,AjaxRemotingProvider,Utils,List,Var,Submitter,View,Concurrency;
 Global=window;
 FsRover=Global.FsRover=Global.FsRover||{};
 Web=FsRover.Web=FsRover.Web||{};
 Client=Web.Client=Web.Client||{};
 WebSharper=Global.WebSharper;
 UI=WebSharper&&WebSharper.UI;
 Next=UI&&UI.Next;
 Doc=Next&&Next.Doc;
 AttrProxy=Next&&Next.AttrProxy;
 Remoting=WebSharper&&WebSharper.Remoting;
 AjaxRemotingProvider=Remoting&&Remoting.AjaxRemotingProvider;
 Utils=WebSharper&&WebSharper.Utils;
 List=WebSharper&&WebSharper.List;
 Var=Next&&Next.Var;
 Submitter=Next&&Next.Submitter;
 View=Next&&Next.View;
 Concurrency=WebSharper&&WebSharper.Concurrency;
 Client.Drive=function()
 {
  var forwardButton,leftButton,rightButton,backwardButton,cam;
  function createRow(leftItems,middleItems,rightItems)
  {
   return Doc.Element("div",[AttrProxy.Create("class","row")],[Doc.Element("div",[AttrProxy.Create("class","col-xs-6 col-sm-4")],leftItems),Doc.Element("div",[AttrProxy.Create("class","col-xs-6 col-sm-4")],middleItems),Doc.Element("div",[AttrProxy.Create("class","clearfix visible-xs-block")],[]),Doc.Element("div",[AttrProxy.Create("class","col-xs-6 col-sm-4")],rightItems)]);
  }
  forwardButton=Doc.Button("Forward",[],function()
  {
   (new AjaxRemotingProvider.New()).Sync("FsRover.Web:FsRover.Web.Server.MoveForward:-71292294",[]);
  });
  leftButton=Doc.Button("Left",[],function()
  {
   (new AjaxRemotingProvider.New()).Sync("FsRover.Web:FsRover.Web.Server.MoveBackward:-71292294",[]);
  });
  rightButton=Doc.Button("Right",[],function()
  {
   (new AjaxRemotingProvider.New()).Sync("FsRover.Web:FsRover.Web.Server.MoveRight:-71292294",[]);
  });
  backwardButton=Doc.Button("Backward",[],function()
  {
   (new AjaxRemotingProvider.New()).Sync("FsRover.Web:FsRover.Web.Server.MoveLeft:-71292294",[]);
  });
  cam=Doc.Element("img",[],[]);
  Global.jQuery("html").keydown(function(event)
  {
   var m,$1,$2,$3,$4;
   {
    m=event.which,($1=Client.IsKey(37,m),$1!=null&&$1.$==1)?(new AjaxRemotingProvider.New()).Sync("FsRover.Web:FsRover.Web.Server.MoveLeft:-71292294",[]):($2=Client.IsKey(40,m),$2!=null&&$2.$==1)?(new AjaxRemotingProvider.New()).Sync("FsRover.Web:FsRover.Web.Server.MoveBackward:-71292294",[]):($3=Client.IsKey(38,m),$3!=null&&$3.$==1)?(new AjaxRemotingProvider.New()).Sync("FsRover.Web:FsRover.Web.Server.MoveForward:-71292294",[]):($4=Client.IsKey(39,m),$4!=null&&$4.$==1)&&(new AjaxRemotingProvider.New()).Sync("FsRover.Web:FsRover.Web.Server.MoveRight:-71292294",[]);
    return;
   }
  });
  Global.setInterval(function()
  {
   var location;
   location=Global.location;
   cam.SetAttribute("src",(function($1)
   {
    return function($2)
    {
     return $1("//"+Utils.toSafe($2)+"/api/photo/reversed/300/240/jpeg");
    };
   }(Global.id))(location.host));
  },150);
  return Doc.Element("div",[],[Doc.Element("h2",[],[Doc.TextNode("Command panel")]),((function(a)
  {
   function c($1,$2)
   {
    return createRow(a,$1,$2);
   }
   return function($1)
   {
    return function($2)
    {
     return c($1,$2);
    };
   };
  }(List.T.Empty))(List.ofArray([forwardButton])))(List.T.Empty),((function(a)
  {
   function c($1,$2)
   {
    return createRow(a,$1,$2);
   }
   return function($1)
   {
    return function($2)
    {
     return c($1,$2);
    };
   };
  }(List.ofArray([leftButton])))(List.ofArray([cam])))(List.ofArray([rightButton])),((function(a)
  {
   function c($1,$2)
   {
    return createRow(a,$1,$2);
   }
   return function($1)
   {
    return function($2)
    {
     return c($1,$2);
    };
   };
  }(List.T.Empty))(List.ofArray([backwardButton])))(List.T.Empty)]);
 };
 Client.Main=function()
 {
  var rvInput,submit,vReversed;
  rvInput=Var.Create$1("");
  submit=Submitter.CreateOption(rvInput.v);
  vReversed=View.MapAsync(function(a)
  {
   var b;
   return a!=null&&a.$==1?(new AjaxRemotingProvider.New()).Async("FsRover.Web:FsRover.Web.Server.DoSomething:-1287498065",[a.$0]):(b=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   }));
  },submit.view);
  return Doc.Element("div",[],[Doc.Input([],rvInput),Doc.Button("Send",[],function()
  {
   submit.Trigger();
  }),Doc.Element("hr",[],[]),Doc.Element("h4",[AttrProxy.Create("class","text-muted")],[Doc.TextNode("The server responded:")]),Doc.Element("div",[AttrProxy.Create("class","jumbotron")],[Doc.Element("h1",[],[Doc.TextView(vReversed)])])]);
 };
 Client.log=function(s)
 {
  Global["eval"]((function($1)
  {
   return function($2)
   {
    return $1("console.log(\""+Utils.toSafe($2)+"\");");
   };
  }(Global.id))(s));
 };
 Client.IsKey=function(k,v)
 {
  return k===v?{
   $:1,
   $0:null
  }:null;
 };
}());
