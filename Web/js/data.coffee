exports = window.data = {}

createScore = () ->
  Math.round(Math.random()*10)
createCount = () ->
  Math.round(Math.random()*20)

exports.getUser = (userId) ->
  if userId then args = {uid: userId} else args = {}
  $.getJSON 'ajax.ashx?action=loaduser', args
    .then (result) ->
      id: result.UserID
      name: result.UserName
      avatar100: result.ImgUrl
      markedCount: result.TodayCount


exports.getScore = (userID, startDate, endDate) ->
  args =
    sdate: startDate
    edate: endDate
  if userID then args.uid = userID
  $.getJSON 'ajax.ashx?action=loadscore', args
    .then (result) ->
      selfListObj = {}
      if result.SelfList then for item in result.SelfList
        if not selfListObj[item.AppraiseDate] then selfListObj[item.AppraiseDate] = {}
        if item.WorkloadScore then selfListObj[item.AppraiseDate]['workload'] = item.WorkloadScore
        if item.WorkEfficiencyScore then selfListObj[item.AppraiseDate]['efficiency'] = item.WorkEfficiencyScore
        if item.CooperationScore then selfListObj[item.AppraiseDate]['cooperative'] = item.CooperationScore
      otherListObj = {}
      if result.OthersList then for item in result.OthersList
        if not otherListObj[item.AppraiseDate] then otherListObj[item.AppraiseDate] = {}
        if item.WorkloadScore then otherListObj[item.AppraiseDate]['workload'] = item.WorkloadScore
        if item.WorkEfficiencyScore then otherListObj[item.AppraiseDate]['efficiency'] = item.WorkEfficiencyScore
        if item.CooperationScore then otherListObj[item.AppraiseDate]['cooperative'] = item.CooperationScore


      dayCount: result.DayCount
      selfList: _.map selfListObj, (v, k) ->
        date: k
        score: v
      otherList: _.map otherListObj, (v, k) ->
        date: k
        score: v
      workloadCount: result.WorkloadNum or 0
      efficiencyCount: result.WorkEfficiencyNum or 0
      cooperativeCount: result.CooperationNum or 0
      workloadSum: result.SumWorkload or 0
      efficiencySum: result.SumWorkEfficiency or 0
      cooperativeSum: result.SumCooperation or 0

exports.markUser = (uid, scores) ->
  $.getJSON('ajax.ashx?action=markuser', $.extend(scores, {uid: uid}))