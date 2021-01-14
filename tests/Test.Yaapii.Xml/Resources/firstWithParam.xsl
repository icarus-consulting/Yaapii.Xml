<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:j="http://www.jcabi.com" version="2.0" exclude-result-prefixes="j">
  <xsl:include href="second.xsl"/>
  <xsl:param name='faa' as='xs:integer' select='5'/>
  <xsl:template match="/">
    <result>
      <xsl:call-template name="j:format">
        <xsl:with-param name="value" select="5.67"/>
      </xsl:call-template>
      <number>
        <xsl:value-of select='$faa'/>
      </number>
    </result>
  </xsl:template>
</xsl:stylesheet>