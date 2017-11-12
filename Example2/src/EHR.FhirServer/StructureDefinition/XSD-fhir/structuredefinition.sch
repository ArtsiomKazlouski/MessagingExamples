<?xml version="1.0" encoding="UTF-8"?>
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt2">
  <sch:ns prefix="f" uri="http://hl7.org/fhir"/>
  <sch:ns prefix="h" uri="http://www.w3.org/1999/xhtml"/>
  <!-- 
    This file contains just the constraints for the resource StructureDefinition
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
    <sch:title>StructureDefinition</sch:title>
    <sch:rule context="f:StructureDefinition/f:meta/f:security">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:meta/f:tag">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition">
      <sch:assert test="not(exists(f:contained/f:meta/f:versionId)) and not(exists(f:contained/f:meta/f:lastUpdated))">dom-4: If a resource is contained in another resource, it SHALL not have a meta.versionId or a meta.lastUpdated</sch:assert>
      <sch:assert test="not(exists(for $id in f:contained/*/@id return $id[not(ancestor::f:contained/parent::*/descendant::f:reference/@value=concat('#', $id))]))">dom-3: If the resource is contained in another resource, it SHALL be referred to from elsewhere in the resource</sch:assert>
      <sch:assert test="not(parent::f:contained and f:contained)">dom-2: If the resource is contained in another resource, it SHALL not contain nested Resources</sch:assert>
      <sch:assert test="not(parent::f:contained and f:text)">dom-1: If the resource is contained in another resource, it SHALL not contain any narrative</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:text/f:div">
      <sch:assert test="descendant::text()[normalize-space(.)!=''] or descendant::h:img[@src]">txt-2: The narrative SHALL have some non-whitespace content</sch:assert>
      <sch:assert test="not(descendant-or-self::*[not(local-name(.)=('a', 'abbr', 'acronym', 'b', 'big', 'blockquote', 'br', 'caption', 'cite', 'code', 'colgroup', 'dd', 'dfn', 'div', 'dl', 'dt', 'em', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'hr', 'i', 'img', 'li', 'ol', 'p', 'pre', 'q', 'samp', 'small', 'span', 'strong', 'table', 'tbody', 'td', 'tfoot', 'th', 'thead', 'tr', 'tt', 'ul', 'var'))])">txt-1: The narrative SHALL contain only the basic html formatting elements described in chapters 7-11 (except section 4 of chapter 9) and 15 of the HTML 4.0 standard, &lt;a&gt; elements (either name or href), images and internally contained style attributes</sch:assert>
      <sch:assert test="not(descendant-or-self::*/@*[not(name(.)=('abbr', 'accesskey', 'align', 'alt', 'axis', 'bgcolor', 'border', 'cellhalign', 'cellpadding', 'cellspacing', 'cellvalign', 'char', 'charoff', 'charset', 'cite', 'class', 'colspan', 'compact', 'coords', 'dir', 'frame', 'headers', 'height', 'href', 'hreflang', 'hspace', 'id', 'lang', 'longdesc', 'name', 'nowrap', 'rel', 'rev', 'rowspan', 'rules', 'scope', 'shape', 'span', 'src', 'start', 'style', 'summary', 'tabindex', 'title', 'type', 'valign', 'value', 'vspace', 'width'))])">txt-3: The narrative SHALL contain only the basic html formatting attributes described in chapters 7-11 (except section 4 of chapter 9) and 15 of the HTML 4.0 standard, &lt;a&gt; elements (either name or href), images and internally contained style attributes</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:contained/f:meta/f:security">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:contained/f:meta/f:tag">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:contained/f:meta/f:security">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:contained/f:meta/f:tag">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition">
      <sch:assert test="not(f:type/@value = 'extension') or (f:context and f:contextType)">sdf-5: Если тип 'extension', значит структура должна иметь информацию о контексте</sch:assert>
      <sch:assert test="(f:type/@value = 'abstract') or f:base">sdf-4: Структура обязательно должна иметь базовую структуру, иначе ее тип будет 'abstract'</sch:assert>
      <sch:assert test="not(f:type/@value=('Resource', 'Type')) or f:url/@value=concat('http://hl7.org/fhir/StructureDefinition/', f:name/@value)">sdf-7: Если типом является Resource или Type, url должен начинаться с &quot;http://hl7.org/fhir/StructureDefinition/&quot; и заканчиваться именем</sch:assert>
      <sch:assert test="f:snapshot or f:differential">sdf-6: Структура должна иметь либо дифференциал, либо снапшот (либо оба)</sch:assert>
      <sch:assert test="string-join(for $elementName in f:*[self::f:snapshot or self::f:differential]/f:element[position()&gt;1]/f:path/@value return if (starts-with($elementName, concat($elementName/ancestor::f:element/parent::f:*/f:element[1]/f:path/@value, '.'))) then '' else $elementName,'')=''">sdf-8: В любом снапшоте или дифференциале все элементы за исключением первого имеют путь, начинающийся с пути первого элемента + &quot;.&quot;</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:identifier/f:type">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:identifier/f:type/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:identifier/f:period">
      <sch:assert test="not(exists(f:start)) or not(exists(f:end)) or (f:start/@value &lt;= f:end/@value)">per-1: Если указана, начальная дата ДОЛЖНА быть меньше, чем дата окончания</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:identifier/f:assigner">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:useContext">
      <sch:assert test="count(f:coding[f:primary/@value='true'])&lt;=1">ccc-2: Только один кодинг из набора может быть выбран непосредственно пользователем</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:useContext/f:coding">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:contact/f:telecom">
      <sch:assert test="not(exists(f:value)) or exists(f:system)">cpt-2: Если значение указано, система является обязательной.</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:contact/f:telecom/f:period">
      <sch:assert test="not(exists(f:start)) or not(exists(f:end)) or (f:start/@value &lt;= f:end/@value)">per-1: Если указана, начальная дата ДОЛЖНА быть меньше, чем дата окончания</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:code">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:mapping">
      <sch:assert test="exists(f:uri) or exists(f:name)">sdf-2: Должен иметь name или uri (или оба)</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:snapshot">
      <sch:assert test="count(f:element) &gt;= count(distinct-values(f:element/f:path/@value))">sdf-1: Пути элементов должны быть уникальными - либо нет (LM)</sch:assert>
      <sch:assert test="exists(f:base) or (count(f:element) = count(f:element[exists(f:definition) and exists(f:min) and exists(f:max)]))">sdf-3: Если структура является снапшотом, определение каждого элемента должно содержать формальное определение и кардинальное множество</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:snapshot/f:element">
      <sch:assert test="not(exists(for $type in f:type return $type/preceding-sibling::f:type[f:code/@value=$type/f:code/@value and f:profile/@value = $type/f:profile/@value]))">eld-13: Types must be unique by the combination of code and profile</sch:assert>
      <sch:assert test="count(f:constraint) = count(distinct-values(f:constraint/f:key/@value))">eld-14: Constraints must be unique by key</sch:assert>
      <sch:assert test="not(exists(f:binding)) or (count(f:type/f:code) = 0) or  f:type/f:code/@value=('code','Coding','CodeableConcept','Quantity','Extension', 'string', 'uri')">eld-11: Binding can only be present for coded elements, string, and uri</sch:assert>
      <sch:assert test="not(exists(f:*[starts-with(local-name(.), 'pattern')])) or not(exists(f:*[starts-with(local-name(.), 'value')]))">eld-8: Pattern and value are mutually exclusive</sch:assert>
      <sch:assert test="count(f:constraint[f:name]) = count(distinct-values(f:constraint/f:name/@value))">eld-15: Constraint names must be unique.</sch:assert>
      <sch:assert test="not(exists(f:*[starts-with(local-name(.), 'fixed')])) or not(exists(f:meaningWhenMissing))">eld-16: default value and meaningWhenMissing are mutually exclusive</sch:assert>
      <sch:assert test="(not(f:max/@value) and not(f:min/@value)) or (f:max/@value = '*') or (f:max/@value &gt;= f:min/@value)">eld-2: Min &lt;= Max</sch:assert>
      <sch:assert test="not(exists(f:*[starts-with(local-name(.), 'pattern')])) or (count(f:type)=1 )">eld-7: Pattern may only be specified if there is one type</sch:assert>
      <sch:assert test="not(exists(f:*[starts-with(local-name(.), 'fixed')])) or (count(f:type)=1 )">eld-6: Fixed value may only be specified if there is one type</sch:assert>
      <sch:assert test="not(exists(f:nameReference) and exists(f:*[starts-with(local-name(.), 'value')]))">eld-5: Either a namereference or a fixed value (but not both) is permitted</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:snapshot/f:element/f:code">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:snapshot/f:element/f:slicing">
      <sch:assert test="(f:discriminator) or (f:definition)">eld-1: If there are no discriminators, there must be a definition</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:snapshot/f:element/f:max">
      <sch:assert test="@value='*' or (normalize-space(@value)!='' and normalize-space(translate(@value, '0123456789',''))='')">eld-3: Max SHALL be a number or &quot;*&quot;</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:snapshot/f:element/f:type">
      <sch:assert test="not(exists(f:aggregation)) or exists(f:code[@value = 'Reference'])">eld-4: Aggregation may only be specified if one of the allowed types for the element is a resource</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:snapshot/f:element/f:binding">
      <sch:assert test="not(f:conformance/@value='example' and f:isExtensible/@value='false')">eld-9: Example value sets are always extensible</sch:assert>
      <sch:assert test="(exists(f:valueSetUri) or exists(f:valueSetReference)) or exists(f:description)">eld-10: provide either a reference or a description (or both)</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:snapshot/f:element/f:binding/f:valueSetUri">
      <sch:assert test="starts-with(@value, 'http:') or starts-with(@value, 'https:')">eld-12: uri SHALL start with http:// or https://</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:snapshot/f:element/f:binding/f:valueSetReference">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:differential/f:element">
      <sch:assert test="not(exists(for $type in f:type return $type/preceding-sibling::f:type[f:code/@value=$type/f:code/@value and f:profile/@value = $type/f:profile/@value]))">eld-13: Types must be unique by the combination of code and profile</sch:assert>
      <sch:assert test="count(f:constraint) = count(distinct-values(f:constraint/f:key/@value))">eld-14: Constraints must be unique by key</sch:assert>
      <sch:assert test="not(exists(f:binding)) or (count(f:type/f:code) = 0) or  f:type/f:code/@value=('code','Coding','CodeableConcept','Quantity','Extension', 'string', 'uri')">eld-11: Binding can only be present for coded elements, string, and uri</sch:assert>
      <sch:assert test="not(exists(f:*[starts-with(local-name(.), 'pattern')])) or not(exists(f:*[starts-with(local-name(.), 'value')]))">eld-8: Pattern and value are mutually exclusive</sch:assert>
      <sch:assert test="count(f:constraint[f:name]) = count(distinct-values(f:constraint/f:name/@value))">eld-15: Constraint names must be unique.</sch:assert>
      <sch:assert test="not(exists(f:*[starts-with(local-name(.), 'fixed')])) or not(exists(f:meaningWhenMissing))">eld-16: default value and meaningWhenMissing are mutually exclusive</sch:assert>
      <sch:assert test="(not(f:max/@value) and not(f:min/@value)) or (f:max/@value = '*') or (f:max/@value &gt;= f:min/@value)">eld-2: Min &lt;= Max</sch:assert>
      <sch:assert test="not(exists(f:*[starts-with(local-name(.), 'pattern')])) or (count(f:type)=1 )">eld-7: Pattern may only be specified if there is one type</sch:assert>
      <sch:assert test="not(exists(f:*[starts-with(local-name(.), 'fixed')])) or (count(f:type)=1 )">eld-6: Fixed value may only be specified if there is one type</sch:assert>
      <sch:assert test="not(exists(f:nameReference) and exists(f:*[starts-with(local-name(.), 'value')]))">eld-5: Either a namereference or a fixed value (but not both) is permitted</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:differential/f:element/f:code">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:differential/f:element/f:slicing">
      <sch:assert test="(f:discriminator) or (f:definition)">eld-1: If there are no discriminators, there must be a definition</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:differential/f:element/f:max">
      <sch:assert test="@value='*' or (normalize-space(@value)!='' and normalize-space(translate(@value, '0123456789',''))='')">eld-3: Max SHALL be a number or &quot;*&quot;</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:differential/f:element/f:type">
      <sch:assert test="not(exists(f:aggregation)) or exists(f:code[@value = 'Reference'])">eld-4: Aggregation may only be specified if one of the allowed types for the element is a resource</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:differential/f:element/f:binding">
      <sch:assert test="not(f:conformance/@value='example' and f:isExtensible/@value='false')">eld-9: Example value sets are always extensible</sch:assert>
      <sch:assert test="(exists(f:valueSetUri) or exists(f:valueSetReference)) or exists(f:description)">eld-10: provide either a reference or a description (or both)</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:differential/f:element/f:binding/f:valueSetUri">
      <sch:assert test="starts-with(@value, 'http:') or starts-with(@value, 'https:')">eld-12: uri SHALL start with http:// or https://</sch:assert>
    </sch:rule>
    <sch:rule context="f:StructureDefinition/f:differential/f:element/f:binding/f:valueSetReference">
      <sch:assert test="not(starts-with(f:reference/@value, '#')) or exists(ancestor::f:entry/f:resource/f:*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')]|/*/f:contained/f:*[f:id/@value=substring-after(current()/f:reference/@value, '#')])">ref-1: SHALL have a local reference if the resource is provided inline</sch:assert>
    </sch:rule>
    </sch:pattern>
</sch:schema>
