using System;

[Serializable]
public class GraphicQualityTag : Enumeration
{
	public static readonly GraphicQualityTag GlobalTextureMipmapLimit	= new(nameof(GlobalTextureMipmapLimit),	1L << 0);
	public static readonly GraphicQualityTag AnisotropicFiltering		= new(nameof(AnisotropicFiltering),		1L << 1);
	public static readonly GraphicQualityTag VerticalSync				= new(nameof(VerticalSync),				1L << 2);

	public static readonly GraphicQualityTag UnitShadow					= new(nameof(UnitShadow),				1L << 3);

	public static readonly GraphicQualityTag CameraFarHalf				= new(nameof(CameraFarHalf),			1L << 4);

	public GraphicQualityTag(string _name,long _qualityOption) : base(_name)
	{
		m_QualityOption = _qualityOption;
	}

	private readonly long m_QualityOption = 0L;
	public long QualityOption => m_QualityOption;
}