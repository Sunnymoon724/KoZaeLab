using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace JCLib
{
    public class JoyConMgr : AutoSingletonMB<JoyConMgr>
    {
        // Settings accessible via Unity
        public bool EnableIMU = true;
        public bool EnableLocalize = true;

        // Different operating systems either do or don't like the trailing zero
        private const ushort vendor_id = 0x57e;
        private const ushort vendor_id_ = 0x057e;
        private const ushort product_l = 0x2006;
        private const ushort product_r = 0x2007;

        public List<JoyConLib> controllers = new List<JoyConLib>();

        public List<IJoyCon> datas = new List<IJoyCon>();

		protected override void DoAwake()
        {
			var isLeft = false;
			HIDapi.hid_init();

			var ptr = HIDapi.hid_enumerate(vendor_id,0x0);
			var top_ptr = ptr;

			if (ptr == IntPtr.Zero)
			{
				ptr = HIDapi.hid_enumerate(vendor_id_,0x0);

				if (ptr == IntPtr.Zero)
				{
					HIDapi.hid_free_enumeration(ptr);
					Debug.Log("No Joy-Cons found!");
				}
			}

			hid_device_info enumerate;

			while (ptr != IntPtr.Zero)
			{
				enumerate = (hid_device_info) Marshal.PtrToStructure(ptr,typeof(hid_device_info));

				Debug.Log(enumerate.product_id);

				if (enumerate.product_id == product_l || enumerate.product_id == product_r)
				{
					if (enumerate.product_id == product_l)
					{
						isLeft = true;
						Debug.Log("Left Joy-Con connected.");
					}
					else if (enumerate.product_id == product_r)
					{
						isLeft = false;
						Debug.Log("Right Joy-Con connected.");
					}
					else
					{
						Debug.Log("Non Joy-Con input device skipped.");
					}

					IntPtr handle = HIDapi.hid_open_path(enumerate.path);
					HIDapi.hid_set_nonblocking(handle,1);

					controllers.Add(new JoyConLib(handle,EnableIMU,EnableLocalize & EnableIMU,0.05f,isLeft));
				}

				ptr = enumerate.next;
			}

			HIDapi.hid_free_enumeration(top_ptr);
		}

		void Start()
		{
            for (int i=0;i<controllers.Count;i++)
            {
                var LEDs = (byte)(0x1 << i);

				controllers[i].Attach(LEDs);
				controllers[i].Begin();
			}
		}

		void Update()
		{
            for (int i=0;i<datas.Count;i++)
            {
				if(i<controllers.Count )
                {
					var controller = controllers[i];
					var data = datas[i];

					controller.Update();

					if (controller.GetButtonDown(out var button))
					{
						data.SetButtonDown(button);
					}

					if (controller.GetButtonUp(out button))
					{
						data.SetButtonUp(button);
					}

					if (controller.GetButton(out button))
					{
						data.SetButton(button);
					}

					data.SetStick(controller.GetStick());

					// Gyro values: x, y, z axis values (in radians per second)
					data.SetGyro(controller.GetGyro());

					// Accel values:  x, y, z axis values (in Gs)
					data.SetAccel(controller.GetAccel());

					data.SetOrientation(controller.GetVector());
				}
			}
		}

		protected override void OnApplicationQuit()
		{
			base.OnApplicationQuit();

			foreach (var controller in controllers)
			{
				controller.Detach();
			}
		}

		public int AddJoyCon(IJoyCon _object)
        {
			datas.Add(_object);

			return datas.Count-1;
		}

		public void SetVibration(int _index)
        {
			// Rumble for 200 milliseconds, with low frequency rumble at 160 Hz and high frequency rumble at 320 Hz. For more information check:
			// https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md
			controllers[_index].SetRumble(160,320,0.6f,200);

			// The last argument (time) in SetRumble is optional. Call it with three arguments to turn it on without telling it when to turn off.
			// (Useful for dynamically changing rumble values.)
			// Then call SetRumble(0,0,0) when you want to turn it off.
		}

		public void SetReCenter(int _index)
        {
			// Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
			controllers[_index].Recenter();
		}
	}
}