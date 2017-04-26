<?xml version="1.0"?>
<xsl:stylesheet
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:input="urn:input-variables"
  exclude-result-prefixes="xsl input"
  version="1.1">

  <xsl:output method="xml" indent="yes" encoding="UTF-8" />

  <xsl:param name="input:language"/>

  <xsl:variable name="smallcase" select="'abcdefghijklmnopqrstuvwxyz'" />
  <xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />

  <xsl:template match="para">
    <p>
      <xsl:apply-templates />
    </p>
  </xsl:template>

  <xsl:template match="b">
    <strong>
      <xsl:apply-templates />
    </strong>
  </xsl:template>

  <xsl:template match="i">
    <em>
      <xsl:apply-templates />
    </em>
  </xsl:template>

  <xsl:template match="ui">
    <strong>
      <xsl:apply-templates />
    </strong>
  </xsl:template>

  <xsl:template match="c">
    <code>
      <xsl:apply-templates />
    </code>
  </xsl:template>

  <xsl:template match="code">
    <pre>
      <code>
        <xsl:if test="normalize-space(@language)">
          <xsl:attribute name="class">
            <xsl:value-of select="@language" />
          </xsl:attribute>
        </xsl:if>
        <xsl:apply-templates />
      </code>
    </pre>
  </xsl:template>

  <xsl:template match="value">
    <returns>
      <xsl:apply-templates />
    </returns>
  </xsl:template>

  <xsl:template match="see[@langword]">
    <xsl:value-of select="concat('`', @langword, '`')"/>
  </xsl:template>

  <xsl:template match="see[@cref and not(parent::member)]">
    <xsl:choose>
      <xsl:when test="contains(normalize-space(@cref), 'Overload:')">
        <xref href="@cref">
          <xsl:attribute name="href">
            <xsl:value-of select="concat(substring-after(@cref, ':'), '*')"/>
          </xsl:attribute>
        </xref>
      </xsl:when>
      <xsl:when test="contains(normalize-space(@cref), ':')">
        <xref href="@cref">
          <xsl:attribute name="href">
            <xsl:value-of select="substring-after(@cref, ':')"/>
          </xsl:attribute>
        </xref>
      </xsl:when>
      <xsl:otherwise>
        <xref href="@cref">
          <xsl:attribute name="href">
            <xsl:value-of select="normalize-space(@cref)"/>
          </xsl:attribute>
        </xref>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="see[@href and not(parent::member)]">
    <a>
      <xsl:apply-templates select="@*|node()"/>
    </a>
  </xsl:template>

  <xsl:template match="seealso[@href and not(parent::member)]">
    <a>
      <xsl:apply-templates select="@*|node()"/>
    </a>
  </xsl:template>

  <xsl:template match="paramref">
    <xsl:if test="normalize-space(@name)">
      <code>
        <xsl:value-of select="@name" />
      </code>
    </xsl:if>
  </xsl:template>

  <xsl:template match="typeparamref">
    <xsl:if test="normalize-space(@name)">
      <code>
        <xsl:value-of select="@name" />
      </code>
    </xsl:if>
  </xsl:template>

  <xsl:template match="list">
    <xsl:variable name="listtype">
      <xsl:value-of select="normalize-space(@type)"/>
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="$listtype = 'table'">
        <table>
          <xsl:if test="listheader">
            <thead>
              <tr>
                <th>
                  <xsl:apply-templates select="listheader/term" />
                </th>
                <th>
                  <xsl:apply-templates select="listheader/description" />
                </th>
              </tr>
            </thead>
          </xsl:if>
          <tbody>
            <xsl:for-each select="item">
              <tr>
                <td>
                  <xsl:apply-templates select="term"/>
                </td>
                <td>
                  <xsl:apply-templates select="description"/>
                </td>
              </tr>
            </xsl:for-each>
          </tbody>
        </table>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="listheader">
          <p>
            <strong>
              <xsl:if test="listheader/term">
                <xsl:value-of select="concat(string(listheader/term),'-')"/>
              </xsl:if>
              <xsl:value-of select="string(listheader/description)" />
            </strong>
          </p>
        </xsl:if>
        <xsl:choose>
          <xsl:when test="$listtype = 'bullet'">
            <ul>
              <xsl:for-each select="item">
                <li>
                  <xsl:apply-templates select="term" />
                  <xsl:apply-templates select="description" />
                </li>
              </xsl:for-each>
            </ul>
          </xsl:when>
          <xsl:when test="$listtype = 'number'">
            <ol>
              <xsl:for-each select="item">
                <li>
                  <xsl:apply-templates select="term" />
                  <xsl:apply-templates select="description" />
                </li>
              </xsl:for-each>
            </ol>
          </xsl:when>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="description">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="term">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>