using System;
using System.Collections.Generic;
using Godot;
using Warlord.Utilities.Extensions;

namespace Warlord.City.Generation
{
    public class StreamLines
    {
        private readonly List<List<Vector2>> _allStreamlines = new();

        private readonly Integrator _integrator;

        private readonly GridStorage _major;
        private readonly GridStorage _minor;
        private readonly Vector2 _origin;
        private readonly StreamLinesParams _parameters;
        private readonly List<List<Vector2>> _streamlinesMajor = new();
        private readonly List<List<Vector2>> _streamlinesMinor = new();
        private readonly Vector2 _worldDimensions;
        private StreamLinesParams _parametersSq;
        public List<List<Vector2>> allStreamlinesSimple = new();

        public StreamLines(Integrator integrator, Vector2 worldDimensions, Vector2 origin, StreamLinesParams parameters)
        {
            _integrator = integrator;
            _worldDimensions = worldDimensions;
            _origin = origin;
            _parameters = parameters;

            if (_parameters.Step > _parameters.Separation)
                GD.PrintErr("Distancia de la muestra de linea de flujo más grande que dsep");

            _major = new GridStorage(_worldDimensions, _origin, _parameters.Separation);
            _minor = new GridStorage(_worldDimensions, _origin, _parameters.Separation);

            SetParametersSq();
        }

        private void SetParametersSq()
        {
            _parametersSq = new StreamLinesParams
            (
                _parameters.Separation * _parameters.Separation,
                _parameters.Test * _parameters.Test,
                _parameters.Step * _parameters.Step,
                _parameters.Lookahead * _parameters.Lookahead,
                _parameters.CircleJoin * _parameters.CircleJoin,
                _parameters.JoinAngle * _parameters.JoinAngle,
                _parameters.PathIterations * _parameters.PathIterations,
                _parameters.SeedTries * _parameters.SeedTries,
                _parameters.SimplifyTolerance * _parameters.SimplifyTolerance,
                _parameters.ColliderEarly * _parameters.ColliderEarly
            );
        }

        public void AddExistingStreamlines(StreamLines streamLines)
        {
            _major.AddAll(streamLines._major);
            _minor.AddAll(streamLines._minor);
        }

        public void CreateAllStreamLines()
        {
            var major = true;
            while (CreateStreamLine(major)) major = !major;

            JoinDanglingStreamlines();
        }

        private Vector2? GetBestNextPoint(Vector2 point, Vector2 previousPoint)
        {
            var nearbyPoints = _major.GetNearbyPoints(point, _parameters.Lookahead);
            nearbyPoints.AddRange(_minor.GetNearbyPoints(point, _parameters.Lookahead));
            var direction = point - previousPoint;

            Vector2? closestSample = null;
            var closestDistance = float.PositiveInfinity;

            foreach (var sample in nearbyPoints)
                if (!sample.Equals(point) && !sample.Equals(previousPoint))
                {
                    var differenceVector = sample - point;
                    var dotDifferenceVector = differenceVector.Dot(direction);
                    if (dotDifferenceVector < 0)
                        continue;

                    var distanceToSample = point.DistanceSquaredTo(sample);
                    if (distanceToSample < closestDistance && distanceToSample < 2 * 0.01f * 0.01f)
                    {
                        closestDistance = distanceToSample;
                        closestSample = sample;
                        continue;
                    }

                    var angleBetween = MathF.Abs(direction.AngleTo(differenceVector));

                    if (angleBetween < _parameters.JoinAngle && distanceToSample < closestDistance)
                    {
                        closestDistance = distanceToSample;
                        closestSample = sample;
                    }
                }

            if (closestSample.HasValue) closestSample += direction.Normalized() * _parameters.SimplifyTolerance * 4;

            return closestSample;
        }

        private void JoinDanglingStreamlines()
        {
            var dstep = 0.01f;
            foreach (var major in new[] { true, false })
            foreach (var streamline in Streamlines(major))
            {
                if (streamline[0].Equals(streamline[^1])) continue;

                var newStart = GetBestNextPoint(streamline[0], streamline[4]);

                if (newStart.HasValue)
                    foreach (var p in PointsBetween(streamline[0], newStart.Value, dstep))
                    {
                        streamline.Insert(0, p);
                        Grid(major).AddSample(p);
                    }

                var newEnd = GetBestNextPoint(streamline[^1], streamline[^4]);

                if (newEnd.HasValue)
                    foreach (var p in PointsBetween(streamline[^1], newEnd.Value, dstep))
                    {
                        streamline.Add(p);
                        Grid(major).AddSample(p);
                    }
            }

            allStreamlinesSimple = new List<List<Vector2>>();
            foreach (var s in _allStreamlines) allStreamlinesSimple.Add(SimplifyStreamline(s));
        }

        private List<Vector2> PointsBetween(Vector2 v1, Vector2 v2, float step)
        {
            var d = v1.DistanceTo(v2);
            var nPoints = Mathf.Floor(d / step);

            if (nPoints == 0) return new List<Vector2>();

            var stepVector = v2 - v1;

            var outVectors = new List<Vector2>();

            var i = 1;
            var next = v1 + stepVector * (i / nPoints);

            for (i = 1; i <= nPoints; i++)
            {
                if (_integrator.Integrate(next, true).LengthSquared() > 0.001f)
                    outVectors.Add(next);
                else
                    return outVectors;

                next = v1 + stepVector * (i / nPoints);
            }

            return outVectors;
        }

        private List<List<Vector2>> Streamlines(bool major)
        {
            return major ? _streamlinesMajor : _streamlinesMinor;
        }

        private GridStorage Grid(bool major)
        {
            return major ? _major : _minor;
        }

        private Vector2 SamplePoint()
        {
            return new Vector2(GD.Randf() * _worldDimensions.X, GD.Randf() * _worldDimensions.Y) + _origin;
        }

        private Vector2? GetSeed(bool major)
        {
            var seed = SamplePoint();
            var i = 0;

            while (!IsValidSample(major, seed, _parametersSq.Separation))
            {
                if (i >= _parameters.SeedTries) return null;

                seed = SamplePoint();
                i++;
            }

            return seed;
        }

        private bool IsValidSample(bool major, Vector2 point, float dSq, bool bothGrids = false)
        {
            var gridValid = Grid(major).IsValidSample(point, dSq);

            if (bothGrids) gridValid = gridValid && Grid(!major).IsValidSample(point, dSq);

            return gridValid;
        }

        private bool CreateStreamLine(bool major)
        {
            var seed = GetSeed(major);
            var isValid = false;
            if (seed.HasValue)
            {
                var streamline = _IntegrateStreamline(seed.Value, major);
                if (ValidStreamline(streamline))
                {
                    Grid(major).AddPolyline(streamline);
                    Streamlines(major).Add(streamline);
                    _allStreamlines.Add(streamline);
                    allStreamlinesSimple.Add(SimplifyStreamline(streamline));
                }

                isValid = true;
            }

            return isValid;
        }

        private List<Vector2> SimplifyStreamline(List<Vector2> streamline)
        {
            var simplified = new List<Vector2>();

            foreach (var point in Vector2Extensions.Simplify(streamline.ToArray(), _parameters.SimplifyTolerance))
                simplified.Add(new Vector2((float)point.X, (float)point.Y));

            return simplified;
        }

        private bool ValidStreamline(List<Vector2> streamline)
        {
            return streamline.Count > 5;
        }

        private List<Vector2> _IntegrateStreamline(Vector2 seed, bool major)
        {
            var count = 0;
            var pointsEscaped = false;

            var collideBoth = GD.Randf() < _parameters.ColliderEarly;

            var d = _integrator.Integrate(seed, major);

            var forwardParameters = new StreamLineIntegration
            {
                seed = seed,
                originalDir = d,
                streamline = new List<Vector2> { seed },
                previousDirection = d,
                previousPoint = seed + d,
                valid = true
            };

            forwardParameters.valid = PointInBounds(forwardParameters.previousPoint);

            var negD = -d;
            var backwardParameters = new StreamLineIntegration
            {
                seed = seed,
                originalDir = negD,
                streamline = new List<Vector2>(),
                previousDirection = negD,
                previousPoint = seed + negD,
                valid = true
            };

            backwardParameters.valid = PointInBounds(backwardParameters.previousPoint);

            var finished = false;
            while (!finished && count < _parameters.PathIterations && (forwardParameters.valid || backwardParameters.valid))
            {
                StreamLineIntegrationStep(ref forwardParameters, major, collideBoth);
                StreamLineIntegrationStep(ref backwardParameters, major, collideBoth);

                var sqDistanceBetweenPoints =
                    forwardParameters.previousPoint
                        .DistanceSquaredTo(backwardParameters.previousPoint);

                if (!pointsEscaped && sqDistanceBetweenPoints > _parametersSq.CircleJoin) pointsEscaped = true;

                if (pointsEscaped && sqDistanceBetweenPoints <= _parametersSq.CircleJoin)
                {
                    forwardParameters.streamline.Add(forwardParameters.previousPoint);
                    forwardParameters.streamline.Add(backwardParameters.previousPoint);
                    backwardParameters.streamline.Add(backwardParameters.previousPoint);
                    finished = true;
                }

                count++;
            }

            backwardParameters.streamline.Reverse();
            backwardParameters.streamline.AddRange(forwardParameters.streamline);
            return backwardParameters.streamline;
        }

        private void StreamLineIntegrationStep(ref StreamLineIntegration parameters, bool major, bool collideBoth)
        {
            if (!parameters.valid) return;

            parameters.streamline.Add(parameters.previousPoint);
            var nextDirection = _integrator.Integrate(parameters.previousPoint, major);

            if (nextDirection.LengthSquared() < 0.01f)
            {
                parameters.valid = false;
                return;
            }

            if (nextDirection.Dot(parameters.previousDirection) < 0) nextDirection *= -1;

            var nextPoint = parameters.previousPoint + nextDirection;

            if (PointInBounds(nextPoint)
                && IsValidSample(major, nextPoint, _parametersSq.Test, collideBoth)
                && !StreamlineTurned(parameters.seed, parameters.originalDir, nextPoint, nextDirection))
            {
                parameters.previousPoint = nextPoint;
                parameters.previousDirection = nextDirection;
            }
            else
            {
                parameters.streamline.Add(nextPoint);
                parameters.valid = false;
            }
        }

        private static bool StreamlineTurned(Vector2 seed, Vector2 originalDir, Vector2 point, Vector2 direction)
        {
            if (!(originalDir.Dot(direction) < 0))
                return false;

            var perpendicularVector = new Vector2(originalDir.Y, -originalDir.X);
            var isLeft = (point - seed).Dot(perpendicularVector) < 0;
            var directionUp = direction.Dot(perpendicularVector) > 0;

            return isLeft == directionUp;
        }

        private bool PointInBounds(Vector2 v)
        {
            return v.X >= _origin.X
                   && v.Y >= _origin.Y
                   && v.X < _worldDimensions.X + _origin.X
                   && v.Y < _worldDimensions.Y + _origin.Y;
        }
    }

    public struct StreamLinesParams
    {
        public Single Separation { get; private set; }
        public Single Test { get; private set; }
        public Single Step { get; private set; }
        public Single Lookahead { get; private set; }
        public Single CircleJoin { get; private set; }
        public Single JoinAngle { get; private set; }
        public Single PathIterations { get; private set; }
        public Single SeedTries { get; private set; }
        public Single SimplifyTolerance { get; private set; }
        public Single ColliderEarly { get; private set; }


        public StreamLinesParams(Single separation, Single test, Single step, Single lookahead, Single circleJoin, Single joinAngle, Single pathIterations, Single seedTries, Single simplifyTolerance, Single colliderEarly)
        {
            Separation = separation;
            Test = Mathf.Min(test, Separation);
            Step = step;
            Lookahead = lookahead;
            CircleJoin = circleJoin;
            JoinAngle = joinAngle;
            PathIterations = pathIterations;
            SeedTries = seedTries;
            SimplifyTolerance = simplifyTolerance;
            ColliderEarly = colliderEarly;
        }


        public StreamLinesParams SetSeparation(Single separation)
        {
            Separation = separation;
            return this;
        }

        public StreamLinesParams SetLookahead(Single lookahead)
        {
            Lookahead = lookahead;
            return this;
        }

        public static StreamLinesParams Default => new StreamLinesParams()
        {
            Separation = 100f,
            Test = 50f,
            Step = 1,
            Lookahead = 500,
            CircleJoin = 5000,
            JoinAngle = 0.1f,
            PathIterations = 500,
            SeedTries = 300,
            SimplifyTolerance = 0.0125f,
            ColliderEarly = 0
        };
    }

    public struct StreamLineIntegration
    {
        public Vector2 seed;
        public Vector2 originalDir;
        public List<Vector2> streamline;
        public Vector2 previousDirection;
        public Vector2 previousPoint;
        public bool valid;
    }
}
