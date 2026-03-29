using UnityEngine;

public readonly struct UnitStat
{
	public float StartValue { get; init; }

	public float AddValue { get; init; }
	public float RateValue { get; init; }

	public UnitStat(float startValue,float addValue = 0.0f,float rateValue = 1.0f)
	{
		StartValue = startValue;

		AddValue = addValue;
		RateValue = rateValue;
	}

	public float TotalValue => (StartValue+AddValue)*RateValue;

	public UnitStat WithAdd(float newAdd)
	{
		return new(StartValue,newAdd,RateValue);
	}

	public UnitStat WithRate(float newRate)
	{
		return new(StartValue,AddValue,newRate);
	}
}