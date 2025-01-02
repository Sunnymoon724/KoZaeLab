using KZLib.KZUtility;

public class GraphicQualityType : CustomTag
{
	public static readonly GraphicQualityType GlobalTextureMipmapLimit	= new(nameof(GlobalTextureMipmapLimit),	1L << 0);
	public static readonly GraphicQualityType AnisotropicFiltering		= new(nameof(AnisotropicFiltering),		1L << 1);
	public static readonly GraphicQualityType VerticalSync				= new(nameof(VerticalSync),				1L << 2);

	public static readonly GraphicQualityType UnitShadow				= new(nameof(UnitShadow),				1L << 3);

	public static readonly GraphicQualityType CameraFarHalf				= new(nameof(CameraFarHalf),			1L << 4);

	public GraphicQualityType(string name,long qualityOption) : base(name)
	{
		m_qualityOption = qualityOption;
	}

	private readonly long m_qualityOption = 0L;
	public long QualityOption => m_qualityOption;
}