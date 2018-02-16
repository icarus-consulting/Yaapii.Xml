<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema' version='2.0'>
  <xsl:output method='text'  />
  <xsl:param name='boom' />
  <xsl:template match='/'>[<xsl:value-of select='$boom'/>]</xsl:template>
</xsl:stylesheet>