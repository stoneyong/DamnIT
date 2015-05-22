exports = window.data = {}

createScore = () ->
  Math.round(Math.random()*10)
createCount = () ->
  Math.round(Math.random()*20)

exports.getUser = (userId) ->
  if not userId
    $.getJSON 'me.json'
      .then (result) ->
        result
  else
    $.getJSON 'user.json'
      .then (result) ->
        result

exports.getScore = (userID, startDate, endDate) ->
  start = +(startDate.split('.')[startDate.split('.').length - 1])
  end = +(endDate.split('.')[endDate.split('.').length - 1])
  days = (('2014.08.' + day) for day in [start..end])
  selfList = _.map days, (date) ->
    date: date
    score:
      workload: createScore()
      efficiency: createScore()
      cooperative: createScore()
  otherList = _.map days, (date) ->
    date: date
    score:
      workload: createScore()
      efficiency: createScore()
      cooperative: createScore()
  workloadCount = createCount()
  efficiencyCount = createCount()
  cooperativeCount = createCount()

  $.Deferred().resolve
    dayCount: days.length
    selfList: selfList
    otherList: otherList
    workloadCount: workloadCount
    efficiencyCount: efficiencyCount
    cooperativeCount: cooperativeCount
    workloadSum: _.reduce otherList, ((memo, item) -> memo + item.score.workload), 0
    efficiencySum: _.reduce otherList, ((memo, item) -> memo + item.score.efficiency), 0
    cooperativeSum: _.reduce otherList, ((memo, item) -> memo + item.score.cooperative), 0



exports.markUser = (uid, scores) ->
      