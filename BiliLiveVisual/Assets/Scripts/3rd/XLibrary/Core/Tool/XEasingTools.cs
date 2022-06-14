
using System;

namespace XLibrary
{
	/// <summary>
	/// 缓动函数
	/// </summary>

	public static class XEasingTools
	{
		/** Linear */
		public static float Linear(float from, float to, float time)
		{
			return from+(to-from) * time;
		}


		/** Quadratic */
		public static float QuadIn(float from, float to, float time)
		{
			return from+(to-from) * time * time;
		}

		public static float QuadOut(float from, float to, float time)
		{
			return from+(to-from) * time * (2.0f - time);
		}

		public static float QuadBoth(float from, float to, float time)
		{
			if (time < 0.5f)
			{
				return from+(to-from) * time * time * 2.0f;
			}
			else
			{
				return from+(to-from) * (2.0f * time * (2.0f - time) - 1.0f);
			}
		}


		/** Cubic */
		public static float CubicIn(float from, float to, float time)
		{
			return from+(to-from) * time * time * time;
		}

		public static float CubicOut(float from, float to, float time)
		{
			time -= 1.0f;
			return from+(to-from) * (time * time * time + 1.0f);
		}

		public static float CubicBoth(float from, float to, float time)
		{
			if (time < 0.5f)
			{
				return from+(to-from) * 4.0f * time * time * time;
			}
			else
			{
				time -= 1.0f;
				return from+(to-from) * (4.0f * time * time * time + 1.0f);
			}
		}



		/** Quartic */
		public static float QuartIn(float from, float to, float time)
		{
			return from+(to-from) * time * time * time * time;
		}

		public static float QuartOut(float from, float to, float time)
		{
			time -= 1.0f;
			return from+(to-from) * (time * time * time * (-time) + 1.0f);
		}

		public static float QuartBoth(float from, float to, float time)
		{
			if (time < 0.5f)
			{
				return from+(to-from) * 8.0f * time * time * time * time;
			}
			else
			{
				time -= 1.0f;
				return from+(to-from) * (-8.0f * time * time * time * time + 1.0f);
			}
		}



		/** Quintic */
		public static float QuintIn(float from, float to, float time)
		{
			return from+(to-from) * time * time * time * time * time;
		}

		public static float QuintOut(float from, float to, float time)
		{
			time -= 1.0f;
			return from+(to-from) * (time * time * time * time * time + 1.0f);
		}

		public static float QuintBoth(float from, float to, float time)
		{
			if (time < 0.5f)
			{
				return from+(to-from) * 16.0f * time * time * time * time * time;
			}
			else
			{
				time -= 1.0f;
				return from+(to-from) * (16.0f * time * time * time * time * time + 1.0f);
			}
		}



		/** Sine */
		public static float SineIn(float from, float to, float time)
		{
			return from+(to-from) * (1.0f - (float)Math.Cos(time * 2f*Math.PI));
		}

		public static float SineOut(float from, float to, float time)
		{
			return from+(to-from) * (float)Math.Sin(time * 2f*Math.PI);
		}

		public static float SineBoth(float from, float to, float time)
		{
			return from+(to-from) * 0.5f * (1.0f - (float)Math.Cos(time * Math.PI));
		}


		/* exponential */
		public static float ExpoIn(float from, float to, float time)
		{
			if (time <= 0.0f)
			{
				return from;
			}
			else
			{
				return from+(to-from) * (float)Math.Pow(2.0f, 10.0f * (time - 1.0f));
			}
		}

		public static float ExpoOut(float from, float to, float time)
		{
			if (time >= 1.0f)
			{
				return to;
			}
			else
			{
				return from+(to-from) * (1.0f - (float)Math.Pow(2.0f, -10.0f * time));
			}
		}

		public static float ExpoBoth(float from, float to, float time)
		{
			if (time <= 0.0f)
			{
				return from;
			}

			if (time >= 1.0f)
			{
				return to;
			}

			if (time < 0.5f)
			{
				return from+(to-from) * 0.5f * (float)Math.Pow(2.0f, 20.0f * time - 10.0f);
			}
			else
			{
				return from+(to-from) * 0.5f * (2.0f - (float)Math.Pow(2.0f, -20.0f * time + 10.0f));
			}
		}


		/** Circular */
		public static float CircIn(float from, float to, float time)
		{
			return from+(to-from) * (1.0f - (float)Math.Sqrt(1.0f - time * time));
		}

		public static float CircOut(float from, float to, float time)
		{
			return from+(to-from) * (float)Math.Sqrt((2.0f - time) * time);
		}

		public static float CircBoth(float from, float to, float time)
		{
			if (time < 0.5f)
			{
				return from+(to-from) * 0.5f * (1.0f - (float)Math.Sqrt(1.0f - 4.0 * time * time));
			}
			else
			{
				time = time * 2.0f - 2.0f;
				return from+(to-from) * 0.5f * ((float)Math.Sqrt(1.0f - time * time) + 1);
			}
		}

		/** Elastic */
		public static float ElasticIn(float from, float to, float time)
		{
			if (time <= 0.0f)
			{
				return from;
			}

			if (time >= 1.0f)
			{
				return to;
			}

			return from+(to-from) * -(float)Math.Pow(2.0f, 10.0f * time - 10.0f) * (float)Math.Sin((3.33f * time - 3.58f) * 2f*Math.PI);
		}

		public static float ElasticOut(float from, float to, float time)
		{
			if (time <= 0.0f)
			{
				return from;
			}

			if (time >= 1.0f)
			{
				return to;
			}

			return from+(to-from) * ((float)Math.Pow(2.0f, -10.0f * time) * (float)Math.Sin((3.33f * time - 0.25f) * 2f*Math.PI) + 1.0f);
		}

		public static float ElasticBoth(float from, float to, float time)
		{
			if (time <= 0.0f)
			{
				return from;
			}

			if (time >= 1.0f)
			{
				return to;
			}

			if (time < 0.5f)
			{
				return from+(to-from) * -0.5f * (float)Math.Pow(2.0f, 20.0f * time - 10.0f) * (float)Math.Sin((4.45f * time - 2.475f) * 2f*Math.PI);
			}
			else
			{
				return from+(to-from) * ((float)Math.Pow(2.0f, -20.0f * time + 10.0f) * (float)Math.Sin((4.45f * time - 2.475f) * 2f*Math.PI) * 0.5f + 1.0f);
			}
		}



		/** Back */
		public static float BackIn(float from, float to, float time)
		{
			return from+(to-from) * time * time * (2.70158f * time - 1.70158f);
		}

		public static float BackOut(float from, float to, float time)
		{
			time -= 1.0f;
			return from+(to-from) * (time * time * (2.70158f * time + 1.70158f) + 1.0f);
		}

		public static float BackBoth(float from, float to, float time)
		{
			if (time < 0.5f)
			{
				return from+(to-from) * time * time * (14.379636f * time - 5.189818f);
			}
			else
			{
				time -= 1.0f;
				return from+(to-from) * (time * time * (14.379636f * time + 5.189818f) + 1.0f);
			}
		}

		/** Bounce */
		public static float BounceOut(float from, float to, float time)
		{
			if (time < 0.363636f)
			{
				return from+(to-from) * 7.5625f * time * time;
			}
			else if (time < 0.72727f)
			{
				time -= 0.545454f;
				return from+(to-from) * (7.5625f * time * time + 0.75f);
			}
			else if (time < 0.909091f)
			{
				time -= 0.818182f;
				return from+(to-from) * (7.5625f * time * time + 0.9375f);
			}
			else
			{
				time -= 0.954545f;
				return from+(to-from) * (7.5625f * time * time + 0.984375f);
			}
		}

		public static float BounceIn(float from, float to, float time)
		{
			if (time > 0.636364f)
			{
				time = 1.0f - time;
				return from+(to-from) * (1.0f - 7.5625f * time * time);
			}
			else if (time > 0.27273f)
			{
				time = 0.454546f - time;
				return from+(to-from) * (0.25f - 7.5625f * time * time);
			}
			else if (time > 0.090909f)
			{
				time = 0.181818f - time;
				return from+(to-from) * (0.0625f - 7.5625f * time * time);
			}
			else
			{
				time = 0.045455f - time;
				return from+(to-from) * (0.015625f - 7.5625f * time * time);
			}
		}

		public static float BounceBoth(float from, float to, float time)
		{
			if (time < 0.5f)
			{
				// bounce in
				if (time > 0.318182f)
				{
					time = 1.0f - time * 2.0f;
					return from+(to-from) * (0.5f - 3.78125f * time * time);
				}
				else if (time > 0.136365f)
				{
					time = 0.454546f - time * 2.0f;
					return from+(to-from) * (0.125f - 3.78125f * time * time);
				}
				else if (time > 0.045455f)
				{
					time = 0.181818f - time * 2.0f;
					return from+(to-from) * (0.03125f - 3.78125f * time * time);
				}
				else
				{
					time = 0.045455f - time * 2.0f;
					return from+(to-from) * (0.007813f - 3.78125f * time * time);
				}
			}

			// bounce out
			if (time < 0.681818f)
			{
				time = time * 2.0f - 1.0f;
				return from+(to-from) * (3.78125f * time * time + 0.5f);
			}
			else if (time < 0.863635f)
			{
				time = time * 2.0f - 1.545454f;
				return from+(to-from) * (3.78125f * time * time + 0.875f);
			}
			else if (time < 0.954546f)
			{
				time = time * 2.0f - 1.818182f;
				return from+(to-from) * (3.78125f * time * time + 0.96875f);
			}
			else
			{
				time = time * 2.0f - 1.954545f;
				return from+(to-from) * (3.78125f * time * time + 0.992188f);
			}
		}
	}
}
