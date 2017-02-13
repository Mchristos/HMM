namespace HMM
{
    public class StateObservationPair<S, T>
    {
        public S State { get; set; }
        public T Observation { get; set; }

        public StateObservationPair(S state, T observation)
        {
            State = state;
            Observation = observation;
        }

        public StateObservationPair()
        {
        }

        public static StateObservationPair<S, T> New(S state, T observation)
        {
            var result = new StateObservationPair<S, T>();
            result.State = state;
            result.Observation = observation;
            return result;
        }
    }
}