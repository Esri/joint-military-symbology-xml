<?xml version='1.0' ?>
<xsl:stylesheet 
	version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xs="http://www.w3.org/2001/XMLSchema" 
	exclude-result-prefixes="xs">

	<xsl:output
		method="xml"
		indent="yes"/>
        <xsl:param name="recursive">true</xsl:param>

	<xsl:template match="/">
		<links xmlns="http://titanium.dstc.edu.au/xml/xs3p">
			<xsl:apply-templates select="//xs:include | //xs:import" />
		</links>
	</xsl:template>

	<xsl:template match="xs:include | xs:import">
		<schema>
			<xsl:attribute name="file-location">
				<xsl:value-of select="@schemaLocation"/>
			</xsl:attribute>
			<xsl:attribute name="docfile-location">
				<xsl:value-of select="@schemaLocation"/>.html</xsl:attribute>
		</schema>
                <xsl:if test="normalize-space(translate($recursive, 'TRUE', 'true'))='true'">
		       <xsl:apply-templates select="document(@schemaLocation)/xs:schema/xs:include | document(@schemaLocation)/xs:schema/xs:import" />
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>
