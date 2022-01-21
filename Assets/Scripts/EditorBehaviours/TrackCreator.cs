using System.Collections.Generic;
using System.Linq;
using Tank.Movement;
using UnityEditor;
using UnityEngine;

namespace EditorBehaviours
{
    [ExecuteInEditMode]
    public class TrackCreator : MonoBehaviour
    {
        [SerializeField] private GameObject trackPartPrefab;
        [SerializeField] private Transform[] trackWayPoints;
        [SerializeField] private Transform trackRoot;
        [SerializeField] private float trackLineResolution = 0.1F;
        [SerializeField] private bool drawTrackLine;
        [SerializeField] private float chainDistance = 0.1F;

        private List<Vector3> trackSpawnKeyPoints;
        private List<TrackPart> parts;

        private bool isCreationInProgress;

        public void SpawnTrackChain()
        {
            if (trackPartPrefab == null)
            {
                Debug.Log("Cant create track chain: the part of track chain is not assigned!", this);
                return;
            }

            CreateTrack();
            SetupTrackParts();
        }

        private void SetupTrackParts()
        {
            for (var i = 0; i < parts.Count - 1; i++)
            {
                var currentPart = parts[i];
                var nextPart = parts[i + 1];
                currentPart.Setup(nextPart);
            }

            parts[parts.Count - 1].Setup(parts[0]);
        }

        private void CreateTrack()
        {
            isCreationInProgress = true;
            RemoveOldChain();
            parts = new List<TrackPart>(100);
            var origin = trackSpawnKeyPoints[0];
            var distanceCounter = chainDistance;
            for (int i = 0; i < trackSpawnKeyPoints.Count - 1; i++)
            {
                var from = trackSpawnKeyPoints[i];
                var to = i == trackSpawnKeyPoints.Count - 1 ? trackSpawnKeyPoints[0] : trackSpawnKeyPoints[i + 1];
                var nodesDistance = Vector3.Distance(from, to);
                var direction = Vector3.Normalize(to - from);
                if (i == 0)
                    CreateTrackSegment(origin, direction);
                if (distanceCounter >= nodesDistance)
                {
                    distanceCounter -= nodesDistance;
                    if (distanceCounter <= 0)
                    {
                        var delta = direction * -distanceCounter;
                        var spawnPoint = from + delta;
                        CreateTrackSegment(spawnPoint, direction);
                        distanceCounter += chainDistance;
                    }
                }
                else
                {
                    var nodesDistanceCounter = distanceCounter;
                    do
                    {
                        var delta = direction * nodesDistanceCounter;
                        var spawnPoint = from + delta;
                        CreateTrackSegment(spawnPoint, direction);
                        nodesDistanceCounter += chainDistance;
                    } while (nodesDistanceCounter <= nodesDistance);

                    distanceCounter = nodesDistanceCounter - nodesDistance;
                }
            }

            isCreationInProgress = false;
        }

        private void CreateTrackSegment(Vector3 spawnPoint, Vector3 forwardDirection)
        {
            var segmentT = Instantiate(trackPartPrefab, trackRoot).transform;
            segmentT.gameObject.name = "TrackSegment";
            segmentT.position = spawnPoint;
            segmentT.rotation = Quaternion.FromToRotation(segmentT.forward, forwardDirection);
            parts.Add(segmentT.GetComponent<TrackPart>());
        }

        private void RemoveOldChain()
        {
            foreach (var segment in trackRoot.GetComponentsInChildren<TrackPart>())
            {
                DestroyImmediate(segment.gameObject);
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
                return;
            }

            if (isCreationInProgress) return;
            if (trackWayPoints.Length < 3) return;

            bool spawnPointCleared = false;

            foreach (var point in trackWayPoints)
            {
                if (point == null) return;
                if (point.hasChanged)
                {
                    if (!spawnPointCleared)
                    {
                        trackSpawnKeyPoints = new List<Vector3>();
                        spawnPointCleared = true;
                    }

                    for (var j = 0; j < trackWayPoints.Length; j++)
                        SetupSpawnPointBySpline(j);
                }
            }

            trackSpawnKeyPoints.Add(trackSpawnKeyPoints[0]);
            trackSpawnKeyPoints.Add(trackSpawnKeyPoints[1]);
        }

        private void SetupSpawnPointBySpline(int pos)
        {
            Vector3 p0 = trackWayPoints[ClampListPos(pos - 1)].position;
            Vector3 p1 = trackWayPoints[pos].position;
            Vector3 p2 = trackWayPoints[ClampListPos(pos + 1)].position;
            Vector3 p3 = trackWayPoints[ClampListPos(pos + 2)].position;

            int loops = Mathf.FloorToInt(1f / trackLineResolution);

            for (var i = 1; i < loops; i++)
            {
                var t = i * trackLineResolution;
                var newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
                if (!trackSpawnKeyPoints.Contains(newPos))
                    trackSpawnKeyPoints.Add(newPos);
            }
        }

        private int ClampListPos(int pos)
        {
            if (pos < 0)
                pos = trackWayPoints.Length - 1;
            if (pos > trackWayPoints.Length)
                pos = 1;
            else if (pos > trackWayPoints.Length - 1)
                pos = 0;
            return pos;
        }

        private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (t * t * c) + (t * t * t * d));
            return pos;
        }

        private void OnDrawGizmos()
        {
            if (trackWayPoints.Any(trackWayPoint => trackWayPoint == null)) return;
            if (!drawTrackLine) return;

            for (int i = 1; i < trackSpawnKeyPoints.Count - 1; i++)
            {
                var fromPoint = trackSpawnKeyPoints[i];
                var toPoint = trackSpawnKeyPoints[i + 1];
                Gizmos.color = Color.white;
                Gizmos.DrawLine(fromPoint, toPoint);
            }
            Gizmos.color = Color.white;
        }

        private void OnValidate()
        {
            trackLineResolution = Mathf.Clamp(trackLineResolution, 0.05F, 0.5F);
            if (chainDistance < 0.05F) chainDistance = 0.05F;
            Update();
        }
    }
}