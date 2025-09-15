/*
	This file is part of Bon Voyage /L
		© 2024-2025 LisiasT : http://lisias.net <support@lisias.net>
		© 2018-2024 Maja
		© 2016-2018 RealGecko

	Bon Voyage /L is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Bon Voyage /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Bon Voyage /L. If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using System.Collections.Generic;

using UnityEngine;


namespace BonVoyage.UI
{
	internal class WaypointPlotter
	{
		private readonly GameObject parent;
		private readonly GameObject gameObject;
		private readonly LineRenderer lineRenderer;
		private Vessel vessel;
		private CelestialBody celestialBody;
		private List<Vector3> points;

		public WaypointPlotter(GameObject parent)
		{
			this.parent = parent;
			this.gameObject = new GameObject(this.GetType().AssemblyQualifiedName);
			this.gameObject.layer = 9;
			GameObject.DontDestroyOnLoad(this.gameObject);
			this.lineRenderer = this.gameObject.AddComponent<LineRenderer>();
		}

		public void Update(Vessel vessel)
		{
			this.vessel = vessel;
			if (null == this.vessel)
			{
				this.celestialBody = null;
				this.points = null;
				return;
			}

			this.celestialBody = vessel.mainBody;
			this.createRenderer();
			{
				BVController controller = BonVoyage.Instance.GetControllerOfVessel(vessel);
				this.points = (null != controller)
								? convert(controller.path, this.celestialBody)
								: null
							;
			}
		}

		public void Draw(bool visible)
		{
			if (visible == this.lineRenderer.enabled) return;
			if (visible && null != this.points)
			{
				MapObject target = this.findTarget();
				if (null == target) return;

				this.lineRenderer.transform.SetParent(target.transform);
				this.lineRenderer.transform.localScale = Vector3.zero;
				this.lineRenderer.transform.localPosition = Vector3.zero;
				this.lineRenderer.transform.localEulerAngles = Vector3.zero;
				draw(this.points, this.lineRenderer);

				this.lineRenderer.enabled = true;
			} else if (!visible && this.lineRenderer.enabled)
				this.lineRenderer.enabled = false;
		}

		private MapObject findTarget()
		{
			for (int i = 0; i < MapView.MapCamera.targets.Count; ++i)
				if (this.celestialBody == MapView.MapCamera.targets[i].celestialBody)
					return MapView.MapCamera.targets[i];
			return null;
		}

		private static void draw(List<Vector3> points, LineRenderer lineRenderer)
		{
			lineRenderer.positionCount = points.Count;
			if (0 == points.Count) return;
			for (int i = 0; i < points.Count; ++i)
				lineRenderer.SetPosition(i, points[i]);
		}

		private static List<Vector3> convert(List<PathUtils.WayPoint> wayPoints, CelestialBody mainBody)
		{
			if (null == wayPoints) return new List<Vector3>();

			List<Vector3> r = new List<Vector3>(wayPoints.Count);
			for (int i = 0; i < wayPoints.Count; ++i)
				r.Add(wayPoints[i].toVector3(mainBody));
			return r;
		}

		private void createRenderer()
		{
			this.lineRenderer.enabled = false;
			this.lineRenderer.material = makeMaterial();
			this.lineRenderer.useWorldSpace = true;
			this.lineRenderer.alignment = LineAlignment.View;
			this.lineRenderer.startColor = Color.red;
			this.lineRenderer.endColor = Color.yellow;
			this.lineRenderer.startWidth = 0.01f;
			this.lineRenderer.endWidth = 0.01f;
		}

		private static Material __material = null;
		private static Material makeMaterial()
		{
			if (null == __material)
			{
				Shader s = Shader.Find("Particles/Additive");
				s = s ?? Shader.Find("Legacy Shaders/Particles/Additive");
				s = s ?? Shader.Find("Standard");
				__material = new Material(s);
			}
			return __material;
		}
	}
}

