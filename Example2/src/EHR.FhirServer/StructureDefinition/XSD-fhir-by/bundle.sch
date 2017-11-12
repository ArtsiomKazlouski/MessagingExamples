<?xml version="1.0" encoding="UTF-8"?>
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt2">
  <sch:ns prefix="f" uri="http://hl7.org/fhir"/>
  <sch:ns prefix="h" uri="http://www.w3.org/1999/xhtml"/>
  <!-- 
    This file contains just the constraints for the resource Bundle
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
    <sch:title>Bundle</sch:title>
    <sch:rule context="f:Bundle/f:meta/f:security">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Bundle/f:meta/f:tag">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Bundle">
      <sch:assert test="not(f:entry/f:search) or (f:type/@value = 'searchset')">bdl-2: entry.search указывается только для типа бандла searchset</sch:assert>
      <sch:assert test="not(f:total) or (f:type/@value = 'searchset') or (f:type/@value = 'history')">bdl-1: total заполняется только для типа бандла search или history</sch:assert>
      <sch:assert test="not(f:entry/f:transaction) or (f:type/@value = 'transaction') or (f:type/@value = 'history')">bdl-3: entry.transaction when (and only when) a transaction</sch:assert>
      <sch:assert test="not(f:entry/f:transactionResponse) or (f:type/@value = 'transaction-response')">bdl-4: entry.transactionResponse заполняется тогда и только тогда, когда тип бандла transaction-response</sch:assert>
    </sch:rule>
    <sch:rule context="f:Bundle/f:entry">
      <sch:assert test="f:resource or f:transaction or f:transactionResponse">bdl-5: элемент resource должен быть обязательно, если нет transaction или transactionResponse</sch:assert>
    </sch:rule>
    <sch:rule context="f:Bundle/f:entry/f:resource/f:meta/f:security">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Bundle/f:entry/f:resource/f:meta/f:tag">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Bundle/f:entry/f:resource/f:meta/f:security">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    <sch:rule context="f:Bundle/f:entry/f:resource/f:meta/f:tag">
      <sch:assert test="not (exists(f:valueSet) and exists(f:code)) or exists(f:system)">cod-1: Если указан valueSet, то также необходимо указать и URI системы</sch:assert>
    </sch:rule>
    </sch:pattern>
</sch:schema>
