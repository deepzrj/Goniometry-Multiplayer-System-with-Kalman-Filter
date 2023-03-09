using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Filters {
    public class KalmanFilter {

        private float Q; // measurement noise
        private float R; // environment noise
        private float F;  // factor of real value to previous real value
        private float H;  // factor of measured value to real value

        private float m_State;
        private float m_Covariance;



        public KalmanFilter(float mesurementNoize = 0.125f, float environmentNoize = 0.1f, float RealValueToPreviousRealValue = 1, float MeasuredToRealValue = 1) {
            Q = mesurementNoize;
            R = environmentNoize;
            F = RealValueToPreviousRealValue;
            H = MeasuredToRealValue;
        }

        public float MesurementNoise {
            get {
                return Q;
            }
            set {
                Q = value;
            }
        }

        public float EnvironmentNoize {
            get {
                return R;
            }
            set {
                R = value;
            }
        }

        public float RealValueToPreviousRealValue {
            get {
                return F;
            }
            set {
                F = value;
            }
        }

        public float MeasuredToRealValue {
            get {
                return H;
            }
            set {
                H = value;
            }
        }

        public float State {
            get {
                return m_State;
            }
            set {
                m_State = value;
            }
        }

        public float Covariance {
            get {
                return m_Covariance;
            }
            set {
                m_Covariance = value;
            }
        }

        public void Correct(float data) {
            //time update - prediction
            State = F * State;
            Covariance = F * Covariance * F + Q;

            //measurement update - correction
            var K = H * Covariance / (H * Covariance * H + R);
            State = State + K * (data - H * State);
            Covariance = (1 - K * H) * Covariance;
        }

        public float FilterValue(float data) {
            this.Correct(data);
            return State;
        }
    }
}