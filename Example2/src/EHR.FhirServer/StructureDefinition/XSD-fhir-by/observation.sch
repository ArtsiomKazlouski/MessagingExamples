<?xml version="1.0" encoding="UTF-8"?>
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt2">
  <sch:ns prefix="f" uri="http://hl7.org/fhir"/>
  <sch:ns prefix="h" uri="http://www.w3.org/1999/xhtml"/>
  <!-- 
    This file contains just the constraints for the resource Observation
    It is provided for documentation purposes. When actually validating,
    always use fhir-invariants.sch (because of the way containment works)
    Alternatively you can use this file to build a smaller version of
    fhir-invariants.sch (the contents are identical; only include those 
    resources relevant to your implementation).
  -->
    <sch:pattern>
      <sch:title>Global</sch:title>
      <sch:rule context="f:*">
        <sch:assert test="@value|f:*|h:div">global-1: All FHIR elements must have a @value or children</sch:assert>
      </sch:rule>
    </sch:pattern>
  <sch:pattern>
    <sch:title>Observation</sch:title>
    <sch:rule context="f:Observation/f:meta/f:security">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:meta/f:tag">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation">
      <sch:assert test="not(exists(f:contained/f:meta/f:versionId)) and not(exists(f:contained/f:meta/f:lastUpdated))">dom-4: If a resource is contained in another resource, it SHALL not have a meta.versionId or a meta.lastUpdated</sch:assert>
      <sch:assert test="not(exists(for $id in f:contained/*/@id return $id[not(ancestor::f:contained/parent::*/descendant::f:reference/@value=concat('#', $id))]))">dom-3: If the resource is contained in another resource, it SHALL be referred to from elsewhere in the resource</sch:assert>
      <sch:assert test="not(parent::f:contained and f:contained)">dom-2: If the resource is contained in another resource, it SHALL not contain nested Resources</sch:assert>
      <sch:assert test="not(parent::f:contained and f:text)">dom-1: If the resource is contained in another resource, it SHALL not contain any narrative</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:text/f:div">
      <sch:assert test="descendant::text()[normalize-space(.)!=''] or descendant::h:img[@src]">txt-2: The narrative SHALL have some non-whitespace content</sch:assert>
      <sch:assert test="not(descendant-or-self::*[not(local-name(.)=('a', 'abbr', 'acronym', 'b', 'big', 'blockquote', 'br', 'caption', 'cite', 'code', 'colgroup', 'dd', 'dfn', 'div', 'dl', 'dt', 'em', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'hr', 'i', 'img', 'li', 'ol', 'p', 'pre', 'q', 'samp', 'small', 'span', 'strong', 'table', 'tbody', 'td', 'tfoot', 'th', 'thead', 'tr', 'tt', 'ul', 'var'))])">txt-1: The narrative SHALL contain only the basic html formatting elements described in chapters 7-11 (except section 4 of chapter 9) and 15 of the HTML 4.0 standard, &lt;a&gt; elements (either name or href), images and internally contained style attributes</sch:assert>
      <sch:assert test="not(descendant-or-self::*/@*[not(name(.)=('abbr', 'accesskey', 'align', 'alt', 'axis', 'bgcolor', 'border', 'cellhalign', 'cellpadding', 'cellspacing', 'cellvalign', 'char', 'charoff', 'charset', 'cite', 'class', 'colspan', 'compact', 'coords', 'dir', 'frame', 'headers', 'height', 'href', 'hreflang', 'hspace', 'id', 'lang', 'longdesc', 'name', 'nowrap', 'rel', 'rev', 'rowspan', 'rules', 'scope', 'shape', 'span', 'src', 'start', 'style', 'summary', 'tabindex', 'title', 'type', 'valign', 'value', 'vspace', 'width'))])">txt-3: The narrative SHALL contain only the basic html formatting attributes described in chapters 7-11 (except section 4 of chapter 9) and 15 of the HTML 4.0 standard, &lt;a&gt; elements (either name or href), images and internally contained style attributes</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:contained/f:meta/f:security">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:contained/f:meta/f:tag">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:contained/f:meta/f:security">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:contained/f:meta/f:tag">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation">
      <sch:assert test="not(exists(f:dataAbsentReason)) or (not(exists(*[starts-with(local-name(.), 'value')])))">obs-6: Shall only be present if Observation.value[x] is not present</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:code">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:code/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueQuantity">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueCodeableConcept">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueCodeableConcept/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueRange">
      <sch:assert test="not(exists(f:low/f:value/@value)) or not(exists(f:high/f:value/@value)) or (number(f:low/f:value/@value) &lt;= number(f:high/f:value/@value))">rng-2: If present, low SHALL have a lower value than high</sch:assert>
      <sch:assert test="not(exists(f:low/f:comparator) or exists(f:high/f:comparator))">rng-3: Quantity values cannot have a comparator when used in a Range</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueRange/f:low">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueRange/f:high">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueRatio">
      <sch:assert test="(count(f:numerator) = count(f:denominator)) and ((count(f:numerator) &gt; 0) or (count(f:extension) &gt; 0))">rat-1: Числитель и знаменатель ДОЛЖНЫ быть либо оба присутствовать, либо оба отсутствовать. Если они оба отсутствуют, значит элемент Ratio ДОЛЖЕН содержать расширение</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueRatio/f:numerator">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueRatio/f:denominator">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueSampledData/f:origin">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valueAttachment">
      <sch:assert test="not(exists(f:data)) or exists(f:contentType)">att-1: Если во вложении указан элемент data, НЕОБХОДИМО также указать и contentType</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:valuePeriod">
      <sch:assert test="not(exists(f:start)) or not(exists(f:end)) or (f:start/@value &lt;= f:end/@value)">per-1: Если указана, начальная дата ДОЛЖНА быть меньше, чем дата окончания</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:dataAbsentReason">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:dataAbsentReason/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:interpretation">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:interpretation/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:appliesPeriod">
      <sch:assert test="not(exists(f:start)) or not(exists(f:end)) or (f:start/@value &lt;= f:end/@value)">per-1: Если указана, начальная дата ДОЛЖНА быть меньше, чем дата окончания</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:bodySiteCodeableConcept">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:bodySiteCodeableConcept/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:bodySiteReference">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:method">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:method/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:identifier/f:type">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:identifier/f:type/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:identifier/f:period">
      <sch:assert test="not(exists(f:start)) or not(exists(f:end)) or (f:start/@value &lt;= f:end/@value)">per-1: Если указана, начальная дата ДОЛЖНА быть меньше, чем дата окончания</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:identifier/f:assigner">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:subject">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:specimen">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:performer">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:device">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:encounter">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange">
      <sch:assert test="(exists(f:low) or exists(f:high)or exists(f:text))">obs-3: Должен иметь хотя бы нижнее (элемент low) либо верхнее (элемент high) значение (без указания оператора сравнения - компаратора), либо элемент text</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange/f:low">
      <sch:assert test="not(exists(f:comparator)) or boolean(f:comparator/@value = '&lt;') or boolean(f:comparator/@value = '&lt;=')">obs-4: Low range comparators can only be '&gt;' or '&gt;=' or empty</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange/f:low">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange/f:high">
      <sch:assert test="not(exists(f:comparator)) or boolean(f:comparator/@value = '&gt;') or boolean(f:comparator/@value = '&gt;=')">obs-5: High range comparators can only be '&lt;' or '&lt;=' or empty</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange/f:high">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange/f:meaning">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange/f:meaning/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange/f:age">
      <sch:assert test="not(exists(f:low/f:value/@value)) or not(exists(f:high/f:value/@value)) or (number(f:low/f:value/@value) &lt;= number(f:high/f:value/@value))">rng-2: If present, low SHALL have a lower value than high</sch:assert>
      <sch:assert test="not(exists(f:low/f:comparator) or exists(f:high/f:comparator))">rng-3: Quantity values cannot have a comparator when used in a Range</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange/f:age/f:low">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:referenceRange/f:age/f:high">
      <sch:assert test="not(exists(f:code)) or exists(f:system)">qty-3: Если указан код единицы измерений, ТРЕБУЕТСЯ также указать и систему</sch:assert>
    </sch:rule>
    <sch:rule context="f:Observation/f:related/f:target">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    </sch:pattern>
</sch:schema>
