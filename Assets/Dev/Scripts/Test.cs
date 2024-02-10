using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Scripts.Utils;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

namespace Dev.Scripts
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private int _numbersAmount = 100;
        [SerializeField] private float _velocityModifier = 1;
        [SerializeField] private TestBall _ballPrefab;

        [SerializeField] private BoxCollider _collider;

        [SerializeField] private bool _useThreads;


        private List<TestBall> _balls = new List<TestBall>();

        private NativeArray<Vector3> _position;
        private NativeArray<Vector3> _velocities;

        private Vector3 BoundsSize => _collider.transform.localScale;

        private TransformAccessArray _transformAccessArray;

        private void Awake()
        {
            CreateBalls();
        }

        private void CreateBalls()
        {
            _position = new NativeArray<Vector3>(_numbersAmount, Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(_numbersAmount, Allocator.Persistent);

            for (int i = 0; i < _numbersAmount; i++)
            {
                Vector3 spawnPos = Vector3.zero;

                TestBall ball = Instantiate(_ballPrefab, spawnPos, Quaternion.identity);

                _balls.Add(ball);
                _position[i] = spawnPos;
                _velocities[i] = Random.insideUnitSphere.normalized * _velocityModifier;
            }

            _transformAccessArray = new TransformAccessArray(_balls.Select(x => x.transform).ToArray());
        }

        public void Update()
        {
            /*if (Input.anyKey)
            {
                float vertical = Input.GetAxis("Vertical");
                float horizontal = Input.GetAxis("Horizontal");
                float heightAxis = 0;
                if (Input.GetKey(KeyCode.Space))
                {   
                    heightAxis = 1;
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    heightAxis = -1;
                }

                Vector3 moveVector = Vector3.forward * vertical + Vector3.right * horizontal;
                moveVector.y = heightAxis;
                
                for (int i = 0; i < _numbersAmount; i++)
                {
                    _velocities[i] = moveVector * _velocityModifier;
                }
            }*/

            if (Input.GetKeyDown(KeyCode.F))
            {
                for (int i = 0; i < _numbersAmount; i++)
                {
                    _velocities[i] = Random.insideUnitSphere.normalized * _velocityModifier;
                }
            }


            if (_useThreads)
            {
                MoveWithThreads();
            }
            else
            {
                MoveUsually();
            }
        }

        private void MoveUsually()
        {
            for (var i = 0; i < _balls.Count; i++)
            {
                TestBall ball = _balls[i];

                Vector3 pos = ball.transform.position;

                ball.transform.position += _velocities[i] * (_velocityModifier * Time.deltaTime);

                if (pos.x > BoundsSize.x || pos.x < -BoundsSize.x || pos.y > BoundsSize.y || pos.y < -BoundsSize.y)
                {
                    ball.transform.position = Vector3.zero;
                }

                _position[i] = ball.transform.position;
            }
        }

        private void MoveWithThreads()
        {
            var job = new MoveJob()
            {
                deltaTime = Time.deltaTime,
                position = _position,
                velocity = _velocities,
                BoundsSize = BoundsSize
            };

            JobHandle jobHandle = job.Schedule(_transformAccessArray);

            Profiler.BeginSample("Jobs");
            jobHandle.Complete();
            Profiler.EndSample();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(Vector3.zero, _collider.transform.localScale);
        }

        private void OnDestroy()
        {
            _position.Dispose();
            _velocities.Dispose();
            _transformAccessArray.Dispose();
        }
    }

    struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> velocity;
        public NativeArray<Vector3> position;

        public float deltaTime;
        public Vector3 BoundsSize;

        public void Execute(int i, TransformAccess transform)
        {
            Vector3 pos = position[i];

            transform.position += velocity[i] * deltaTime;

            if (pos.x > BoundsSize.x || pos.x < -BoundsSize.x || pos.y > BoundsSize.y || pos.y < -BoundsSize.y)
            {
                position[i] = Vector3.zero;
            }
            else
            {
                position[i] = transform.position;
            }
        }
    }
}