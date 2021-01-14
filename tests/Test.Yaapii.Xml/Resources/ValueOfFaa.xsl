<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='2.0'>
  <xsl:output method='text'    />
  <xsl:param name='faa' as='xs:integer' select='5'/>
  <xsl:template match='/'>+<xsl:value-of select='$faa'/>+</xsl:template>
</xsl:stylesheet>