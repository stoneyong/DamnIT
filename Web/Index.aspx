﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>
<!DOCTYPE html>
<html>
  <head>
    <title>DamnIT</title>
    <link rel="stylesheet" href="lib/purecss/pure-min.css">
    <link rel="stylesheet" href="lib/jqueryui/jquery-ui-1.11.1/jquery-ui.min.css">
    <link rel="stylesheet" href="lib/artdialog/ui-dialog.css">
    <link rel="stylesheet" href="css/style.css">
  </head>
  <body>
    <div class="header">
      <span class="name">
        <span class="first"></span><span></span>
      </span>
    </div>

    <div class="info clearfix">
      <div class="my">
        <div class="avatar">
          <img src="" alt="avatar">
        </div>
        <ul class="menu">
          <li id="mark-self" class="action">自我评价</li>
          <li id="mark-other" class="action">评价他人</li>
          <li id="view-other" class="action">查看上司</li>
          <li id="view-self" class="action">查看自己</li>
          <li class="marked">今日已评价 0</li>
        </ul>
      </div>
      <div class="radialchart">
        <div class="radialchart-text">
          <div class="otherscore">0</div>
          <div class="totalscore">30</div>
        </div>
        <div class="chart"></div>
      </div>
      <div class="score clearfix">
        <div class="title clearfix">
          <div class="timerange">
            <div>时间段</div>
            <div class="daterange">2014.08.01-2014.08.20</div>
            <div>
              <span class="daycount">0</span>天
            </div>
          </div>
          <ul class="statusicons">
            <li class="statusicon statusicon-good"></li>
            <li class="statusicon statusicon-warning"></li>
            <li class="statusicon statusicon-damnit"></li>
          </ul>
        </div>
        <ul class="score-list">
          <li class="clearfix score-item score-item-workload">
            <div class="score-icon"></div>
            <div class="score-title">工作量</div>
            <div class="score-average">平均分<span class="score-average-num">601</span>分</div>
            <div class="score-detail">
              <span class="score-detail-total">607</span>分/<span class="score-detail-count">100</span>人次
            </div>
          </li>
          <hr>
          <li class="clearfix score-item score-item-efficiency">
            <div class="score-icon"></div>
            <div class="score-title">工作效率</div>
            <div class="score-average">平均分<span class="score-average-num">601</span>分</div>
            <div class="score-detail">
              <span class="score-detail-total">607</span>分/<span class="score-detail-count">100</span>人次
            </div>
          </li>
          <hr>
          <li class="clearfix score-item score-item-cooperative">
            <div class="score-icon"></div>
            <div class="score-title">合作性</div>
            <div class="score-average">平均分<span class="score-average-num">601</span>分</div>
            <div class="score-detail">
              <span class="score-detail-total">607</span>分/<span class="score-detail-count">100</span>人次
            </div>
          </li>
        </ul>
      </div>
    </div>

    <div class="barchart">
      <div class="title clearfix">
        <div class="name">评分曲线图</div>
        <dl class="legend">
          <dt class="self-score"></dt><dd>自我评价</dd>
          <dt class="other-score"></dt><dd>他人评价</dd>
        </dl>
      </div>
      <div class="chart"></div>
      <div class="foot clearfix">
        <div class="dayslider"></div>
        <div class="dayindextext">查看最近<span class="dayindex">7</span>天</div>
      </div>
      <div class="item-selector">☰</div>
      <ul class="item-selector-list">
        <li>全部</li>
        <li data-value="workload">工作量</li>
        <li data-value="efficiency">工作效率</li>
        <li data-value="cooperative">合作性</li>
      </ul>
    </div>

    <script id="tpl-select-user-dialog" type="text/template">
      <div class="select-user-dialog">
        <div class="header clearfix">
          <div class="title">查看上司</div>
          <div class="btn-group">
            <div class="btn-ok">OK</div>
            <div class="btn-close">×</div>
          </div>
        </div>
        <div class="user-selector ui-widget">
          <label>选择用户：<input id="boss-search" class="user-search"></label>
        </div>
      </div>
    </script>

    <script id="tpl-mark-dialog" type="text/template">
      <div class="mark-dialog">
        <div class="header clearfix">
          <div class="title">评价</div>
          <div class="btn-group">
            <div class="btn-ok">OK</div>
            <div class="btn-close">×</div>
          </div>
        </div>

        <div class="user-selector ui-widget">
          <label>选择用户：<input id="user-search" class="user-search"></label>
        </div>

        <ul class="mark-list">
          <li class="mark-item mark-item-workload">
            <div class="mark-name">工作量</div>
            <div class="mark-icon"></div>
            <div class="mark-bar"></div>
          </li>
          <li class="mark-item mark-item-efficiency">
            <div class="mark-name">工作效率</div>
            <div class="mark-icon"></div>
            <div class="mark-bar"></div>
          </li>
          <li class="mark-item mark-item-cooperative">
            <div class="mark-name">合作性</div>
            <div class="mark-icon"></div>
            <div class="mark-bar"></div>
          </li>
        </ul>
      </div>
    </script>

    <script src="lib/underscore/underscore.js"></script>
    <script src="lib/d3/d3.js"></script>
    <script src="lib/d3/radialProgress.js"></script>
    <script src="lib/jquery/jquery-1.11.1.js"></script>
    <script src="lib/jqueryui/jquery-ui-1.11.1/jquery-ui.min.js"></script>
    <script src="lib/artdialog/dialog-plus.js"></script>
    <script src="lib/date-utils/date-utils.js"></script>
    <script src="js/data.js"></script>
    <script src="js/main.js"></script>
  </body>
</html>