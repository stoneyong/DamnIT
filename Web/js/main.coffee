dateToStr = (d) ->
  [d.getFullYear(), (d.getMonth() + 1), d.getDate()].join('.')

getRangeByDays = (days) ->
  [dateToStr(new Date().add({'days': -days})), dateToStr(new Date())]


myUserID = null
currentUser = null
storedStartDate = null
storedEndDate = null

$doc = $(document)
$doc
  .on 'init', ->
    $dayindex = $doc.find('.barchart').find('.dayindex')
    $dayslider = $doc.find('.barchart').find('.dayslider').slider
      min: 1
      max: 30
      value: 7
      slide: (e, ui) ->
        $dayindex.html(ui.value)
      stop: (e, ui) ->
        range = getRangeByDays(ui.value)
        $doc.trigger 'loadScore', [range[0], range[1]]
    $doc.find('.barchart').find('.item-selector')
      .on 'mouseenter', ->
        $doc.find('.barchart')
          .find('.item-selector').css('border-bottom-color', 'white')
          .end()
          .find('.item-selector-list').show()
    $doc.find('.barchart').find('.item-selector-list')
      .on 'click', 'li', ->
        $doc.find('.barchart').find('.item-selector').css('border-bottom-color', '#eaeaea')
        $doc.find('.barchart').find('.item-selector-list').hide()
        $doc.trigger 'loadScore', [null, null, $(@).data('value')]
      .on 'mouseleave', ->
        $doc.find('.barchart').find('.item-selector').css('border-bottom-color', '#eaeaea')
        $doc.find('.barchart').find('.item-selector-list').hide()

    data.getUser().then (user) ->
      myUserID = user.id
      $doc.trigger 'initUser', user

  .on 'initUser', (e, user) ->
    $doc.data('myId', user.id).trigger('loadUser', user)

  .on 'loadUser', (e, user, dontloadScore) ->
    currentUser = user
    name = user.name
    first = name[0]
    remain = name[1..]
    $doc
      .data('userId', user.id)
      .find('.header .name')
        .html('')
        .append($('<span />').html(first).addClass('first'))
        .append($('<span />').html(remain))
      .end()
      .find('.my')
        .find('.avatar img')
          .attr('src', user.avatar100)
        .end()
        .find('.marked')
          .html('今日已评价 ' + user.markedCount)
        .end()
      .end()
    if user.id is myUserID
      $doc.find('.my .menu #view-self').hide()
      $doc.find('.my .menu :not(#view-self)').show()
    else
      $doc.find('.my .menu #view-self').show()
      $doc.find('.my .menu :not(#view-self)').hide()
    unless dontloadScore
      $doc.trigger 'loadScore'

  .on 'loadScore', (e, startDate, endDate, scoreItem) ->
    if not startDate then startDate = storedStartDate or getRangeByDays(7)[0]
    if not endDate then endDate = storedEndDate or getRangeByDays(7)[1]
    storedStartDate = startDate
    storedEndDate = endDate
    data.getScore($doc.data('userId'), startDate, endDate).then (result) ->
      if not result then return
      averageScores = []
      _.each ['cooperative', 'efficiency', 'workload'], (item) ->
        average = Math.round(result[item + 'Sum'] / (result[item + 'Count'] or 1) * 10) / 10
        averageScores.push average
        $doc.find('.score').find('.score-item-' + item)
          .find('.score-detail-total')
            .html(result[item + 'Sum'])
          .end()
          .find('.score-detail-count')
            .html(result[item + 'Count'])
          .end()
          .find('.score-average-num')
            .html(average)
          .end()
      averageScoresSelf = _.map ['cooperative', 'efficiency', 'workload'], (item) ->
        total = _.reduce result.selfList, (memo, day) ->
          memo + day.score[item]
        , 0
        Math.round(total / (result.selfList?.length or 1) * 10) / 10
      otherscoreTotal = Math.round((result.workloadSum / result.workloadCount + result.efficiencySum / result.efficiencyCount + result.cooperativeSum / result.cooperativeCount) * 10) / 10 or 0
      selfscoreTotal = _.reduce averageScoresSelf, ((memo, num) -> memo + num), 0
      if otherscoreTotal is 0 then status = 'good'
      else
        if _.reduce [1..3], ((memo, i) -> (averageScoresSelf[i]/averageScores[i]) > 1.2 or memo), false
          status = 'damnit'
        else if _.reduce [1..3], ((memo, i) -> ((averageScoresSelf[i]/averageScores[i]) > 1.1) or memo), false
          status = 'warning'
        else status = 'good'
      $doc
        .find('.score')
          .find('.timerange')
            .find('.daterange')
              .html(startDate + '-' + endDate)
            .end()
            .find('.daycount')
              .animate({opacity: 0}, 200, -> $doc.find('.score').find('.timerange').find('.daycount').html(result.dayCount))
              .animate({opacity: 1}, 200)
            .end()
          .end()
          .find('.statusicon')
            .removeClass('current')
            .filter('.statusicon-' + status)
              .addClass('current')
            .end()
          .end()
        .end()
        .find('.radialchart-text')
          .find('.otherscore')
            .html(otherscoreTotal)
          .end()
        .end()

      rWidth = 142
      rHeight = 142
      rMargin =
        top: 5
        bottom: 5
        left: 5
        right: 5
      radialChart = d3.select('.radialchart .chart').html('')
        .append('svg')
        .attr('transform', "translate(#{rMargin.left}, #{rMargin.top})")
        .attr('width', rWidth - rMargin.left - rMargin.right)
        .attr('height', rHeight - rMargin.top - rMargin.bottom)

      getRadialChartChild = () ->
        radialChart
          .append('g')
          .attr('width', rWidth - rMargin.left - rMargin.right)
          .attr('height', rHeight - rMargin.top - rMargin.bottom)
      getRadialChartColor = (d, i) ->
        switch i
          when 0 then '#FE6440'
          when 1 then '#60AED0'
          when 2 then '#FEA540'
      getRadialChartColorSelf = (d, i) ->
        switch i
          when 0 then '#a67947'
          when 1 then '#577e8f'
          when 2 then '#a55941'

      arcTween = (current, d, i) ->
        _arc  = d3.svg.arc()
          .innerRadius(10 + 16 * (i+1) - 2)
          .outerRadius(10 + 16 * (i+1) + 2)
          .startAngle(0).endAngle(0)
        (a) ->
          interp = d3.interpolate (current or 0), a
          (t) ->
            _arc.endAngle(- interp(t) / 10 * 2 * Math.PI)()
      arcTweenBack = (current, d, i) ->
        _arc  = d3.svg.arc()
          .innerRadius(10 + 16 * (i+1) - 2)
          .outerRadius(10 + 16 * (i+1) + 2)
          .startAngle(- current / 10 * 2 * Math.PI)
          .endAngle(- current / 10 * 2 * Math.PI)
        (a) ->
          interp = d3.interpolate a, (current or 0)
          (t) ->
            _arc.endAngle(- interp(t) / 10 * 2 * Math.PI)()

      getRadialChartChild()
        .selectAll('.arc-holder')
        .data(averageScores).enter()
        .append('path')
        .attr('transform', "translate(#{rWidth/2}, #{rHeight/2})")
        .attr 'd', d3.svg.arc().
          innerRadius((d, i) -> 10 + 16 * (i+1) - 1).
          outerRadius((d, i) -> 10 + 16 * (i+1) + 1).
          startAngle(0).
          endAngle(2 * Math.PI)
        .attr('fill', '#eaeaea')
      arcOther = getRadialChartChild()
        .selectAll('.arc-other')
        .data(averageScores).enter()
        .append('path')
        .attr('transform', "translate(#{rWidth/2}, #{rHeight/2})")
        .attr 'fill', getRadialChartColor
      arcOther.transition().duration(2000)
        .attrTween 'd', (d, i) -> arcTween(0, d, i)(d)
      arcSelf = getRadialChartChild()
        .selectAll('.arc-self')
        .data(averageScoresSelf).enter()
        .append('path')
        .attr('transform', "translate(#{rWidth/2}, #{rHeight/2})")
        .attr 'fill', getRadialChartColorSelf
      arcPoint = getRadialChartChild()
        .selectAll('circle')
        .data(averageScores).enter()
        .append('circle')
        .attr('r', 6)
        .attr('transform', (d, i) -> "translate(#{rWidth/2}, #{rHeight/2-(10 + 16 * (i+1))})")
        .attr 'fill', getRadialChartColor
        .on 'mouseenter', (d, i) ->
          if not selfscoreTotal then return
          d3.select(this).transition().duration(1000)
            .attr('r', '8')
            .attr 'fill', -> getRadialChartColorSelf(d, i)
          d3.select(arcSelf[0][i]).transition().duration(1000)
            .attrTween 'd', (d) -> arcTween(0, d, i)(d)
        .on 'mouseleave', (d, i) ->
          if not selfscoreTotal then return
          d3.select(this).transition().duration(1000)
            .attr('r', '6')
            .attr 'fill', -> getRadialChartColor(d, i)
          d3.select(arcSelf[0][i]).transition().duration(500)
            .attrTween 'd', (d) -> arcTweenBack(0, d, i)(d)

      barchartSelector = scoreItem
      if barchartSelector then filterScore = (scoreObj) -> scoreObj[barchartSelector]
      else filterScore = (scoreObj) ->
        vals = _.map scoreObj, (val, key) -> val
        total = _.reduce vals, ((memo, num) -> memo + num), 0
        if vals and vals.length then total / vals.length else 0

      margin =
        left: 80
        right: 40
        top: 40
        bottom: 40
      outWidth = $doc.find('.barchart').find('.chart').width()
      outHeight = $doc.find('.barchart').find('.chart').height()
      width = outWidth - margin.left - margin.right
      height = outHeight - margin.top - margin.bottom

      selfX = d3.scale.ordinal()
        .domain(_.map result.selfList, (item) -> item.date)
        .rangeBands([0, width])
      selfY = d3.scale.linear()
        .domain([0, 10])
        .range([height, 0])

      yAxis = d3.svg.axis().scale(selfY).orient('left')

      barChart = d3.select('.barchart .chart').html('')
        .append('svg')
          .attr('width', outWidth)
          .attr('height', outHeight)
        .append('g')
          .attr('transform', "translate(#{margin.left}, #{margin.top})")
      barChartYTicks = barChart.append('g')
        .attr('class', 'y axis')
        .attr('transform', "translate(-10, 0)")
        .call(yAxis)
        .selectAll('line')
        .attr('x1', width)
        .attr('stroke-width', 1)
        .attr('stroke', (d, i) -> if i % 2 then '#f3f3f3' else '#e5e5e5')
      selfBar = barChart.selectAll('.bar')
        .data(result.selfList)
        .enter()
        .append('rect')
        .attr('transform', (d) -> "translate(#{selfX(d.date)}, 0)")
        .attr('width', '20')
        .attr('y', height)
        .attr('height', 0)
      selfBar.transition().duration(1000)
        .attr('height', (d) -> height - selfY(filterScore(d.score)))
        .attr('y', (d) -> selfY(filterScore(d.score)))
      selfBar
        .on 'mouseenter', ->
          d3.select(this).style('fill-opacity', 0.8)
        .on 'mouseout', ->
          d3.select(this).style('fill-opacity', 1)

      linkData = ([v, result.otherList[i + 1]] for v, i in result.otherList)
      otherBallLink = barChart.selectAll('.link')
        .data(linkData)
        .enter()
        .append('line')
        .attr('x1', (d) -> selfX(d[0].date) + 10)
        .attr('y1', (d) -> selfY(filterScore(d[0].score)))
        .attr('x2', (d) -> selfX(d[if d[1] then 1 else 0].date) + 10)
        .attr('y2', (d) -> selfY(filterScore(d[if d[1] then 1 else 0].score)))
        .attr('stroke', '#fff')
      otherBallLink.transition().duration(1000)
        .attr('stroke', '#FEA540')

      othersBall = barChart.selectAll('.ball')
        .data(result.otherList)
        .enter()
        .append('circle')
        .attr('transform', (d) -> "translate(#{selfX(d.date) + 10}, #{selfY(filterScore(d.score))})")
        .attr('r', '0')
      othersBall.transition().duration(1000)
        .attr('r', '10')
      othersBall
        .on 'mouseenter', ->
          d3.select(this).style('fill-opacity', 0.8)
        .on 'mouseout', ->
          d3.select(this).style('fill-opacity', 1)

  .on 'click', '#mark-self,#mark-other', (e) ->
    isSelf = $(@).is('#mark-self')
    dlg = dialog
      skin: 'dialog'
      onclose: ->
        $doc.find('.mark-dialog').find('.user-search').val('')
        $doc.find('.mark-dialog').find('.mark-bar-item').removeClass('highlight').removeClass('selected')
    $content = $ $('#tpl-mark-dialog').html()
    if isSelf
      $content.find('.title').html('自我评价')
      $content.find('.user-selector').hide()
    else
      $content.find('.title').html('评价他人')
      $content.find('.user-selector').show()
    $content.find('.mark-list').find('.mark-bar').each (i, v) ->
      $bar = $(v)
      for i in [1..10]
        $('<div/>')
          .addClass('mark-bar-item')
          .attr('data-score', i)
        .appendTo($bar)
    dlg.content($('<div/>').append($content).html())
    $(dlg.node)
      .find('.user-search').autocomplete
        source: 'ajax.ashx?action=searchuser'
        select: (e, ui) ->
          e.preventDefault()
          $(dlg.node)
            .data('selected', ui.item.value)
            .find('.user-search').val(ui.item.label)
        focus: (e, ui) ->
          e.preventDefault()
          $(dlg.node).find('.user-search').val(ui.item.label)
      .end()
      .on 'click', '.mark-bar-item', ->
        $this = $(@)
        score = $this.data('score')
        $this.parent().find('.mark-bar-item').each (i, v) ->
          $sub = $(v)
          if $sub.data('score') <= score
            $sub.addClass('selected')
          else
            $sub.removeClass('selected')
      .on 'mouseleave', '.mark-dialog .mark-bar-item', ->
        $doc.find('.mark-dialog').find('.mark-bar-item').removeClass('highlight')
      .on 'mouseenter', '.mark-bar-item', ->
        $this = $(@)
        score = $this.data('score')
        $this.parent().find('.mark-bar-item').each (i, v) ->
          $sub = $(v)
          if $sub.data('score') <= score then $sub.addClass('highlight')
      .on 'click', '.mark-dialog .btn-close', ->
        dlg.close()
      .on 'click', '.btn-ok', ->
        uid = if isSelf then myUserID else $(dlg.node).data('selected')
        [workload, efficiency, cooperative] = _.map ['workload', 'efficiency', 'cooperative'], (item) ->
          _.reduce $(dlg.node).find('.mark-item-' + item).find('.mark-bar-item.selected'), (memo, li) ->
            score = $(li).data('score')
            if score and score > memo then score else memo
          , 0
        if uid and workload and efficiency and cooperative
          data.markUser(uid, {workload, efficiency, cooperative}).then (resultList) ->
            result = _.reduce resultList, ((memo, num) -> if num > memo then memo else num) , 1
            if result > 0
              alert '评分成功'
              $doc
                .find('.my')
                  .find('.marked')
                    .html('今日已评价 ' + parseInt($doc.find('.my').find('.marked').html()))
            else alert ('错误码: ' + result)
            dlg.close()
          .fail ->
            alert '请求失败'
        else alert '未选择用户或未打分'
    dlg.showModal()

  .on 'click', '#view-other', ->
    dlg = dialog
      skin: 'dialog'
      content: $('#tpl-select-user-dialog').html()
      onclose: ->
        $(dlg.node).data('selected', null)

    $(dlg.node)
      .find('.user-search').autocomplete
        source: 'ajax.ashx?action=searchuser'
        select: (e, ui) ->
          e.preventDefault()
          $(dlg.node)
            .data('selected', ui.item.value)
            .find('.user-search').val(ui.item.label)
        focus: (e, ui) ->
          e.preventDefault()
          $(dlg.node)
            .find('.user-search').val(ui.item.label)
      .end()
      .on 'click', '.btn-close', ->
        $(dlg.node).find('.user-search').val('')
        dlg.close()
      .on 'click', '.btn-ok', ->
        uid = $(dlg.node).data('selected')
        if uid then data.getUser(uid).then (user) ->
          if not user then return $.Deferred().reject()
          $doc.trigger 'loadUser', user
          dlg.close()
        .fail ->
          alert '请求失败'
        else alert '请选择用户'

    dlg.showModal()
  .on 'click', '#view-self', ->
    $doc.trigger 'init'


      

$ ->
  $doc.trigger 'init'