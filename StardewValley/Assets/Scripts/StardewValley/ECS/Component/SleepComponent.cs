namespace WATP.ECS
{

	public interface ISleepComponent : IComponent,ITransformComponent
	{
		public SleepComponent SleepComponent { get; }
	}

	[System.Serializable]
	public class SleepComponent
	{
		public bool isArea = false;
	}

}