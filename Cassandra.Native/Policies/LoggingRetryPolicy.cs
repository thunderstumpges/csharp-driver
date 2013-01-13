using System.Diagnostics;

namespace Cassandra
{

    /**
     * A retry policy that wraps another policy, logging the decision made by its sub-policy.
     * <p>
     * Note that this policy only log the Ignore and Retry decisions (since
     * Rethrow decisions just amount to propate the cassandra exception). The
     * logging is done at the INFO level.
     */
    public class LoggingRetryPolicy : RetryPolicy
    {

        private readonly RetryPolicy _policy;

        /**
         * Creates a new {@code RetryPolicy} that logs the decision of {@code policy}.
         *
         * @param policy the policy to wrap. The policy created by this constructor
         * will return the same decision than {@code policy} but will log them.
         */
        public LoggingRetryPolicy(RetryPolicy policy)
        {
            this._policy = policy;
        }

        private static ConsistencyLevel CL(ConsistencyLevel cl, RetryDecision decision)
        {
            return decision.RetryConsistencyLevel ?? cl;
        }

        public RetryDecision OnReadTimeout(ConsistencyLevel cl, int requiredResponses, int receivedResponses, bool dataRetrieved, int nbRetry)
        {
            RetryDecision decision = _policy.OnReadTimeout(cl, requiredResponses, receivedResponses, dataRetrieved, nbRetry);
            switch (decision.DecisionType)
            {
                case RetryDecision.RetryDecisionType.Ignore:
                    const string f1 = "Ignoring read timeout (initial consistency: {0}, required responses: {1}, received responses: {2}, data retrieved: {3}, retries: {4})";
                    Trace.TraceInformation(f1, cl, requiredResponses, receivedResponses, dataRetrieved, nbRetry);
                    break;
                case RetryDecision.RetryDecisionType.Retry:
                    const string f2 = "Retrying on read timeout at consistency {0} (initial consistency: {1}, required responses: {2}, received responses: {3}, data retrieved: {4}, retries: {5})";
                    Trace.TraceInformation(f2, CL(cl, decision), cl, requiredResponses, receivedResponses, dataRetrieved, nbRetry);
                    break;
            }
            return decision;
        }

        public RetryDecision OnWriteTimeout(ConsistencyLevel cl, string writeType, int requiredAcks, int receivedAcks, int nbRetry)
        {
            RetryDecision decision = _policy.OnWriteTimeout(cl, writeType, requiredAcks, receivedAcks, nbRetry);
            switch (decision.DecisionType)
            {
                case RetryDecision.RetryDecisionType.Ignore:
                    const string f1 = "Ignoring write timeout (initial consistency: {0}, write type: {1} required acknowledgments: {2}, received acknowledgments: {3}, retries: {4})";
                    Trace.TraceInformation(f1, cl, writeType, requiredAcks, receivedAcks, nbRetry);
                    break;
                case RetryDecision.RetryDecisionType.Retry:
                    const string f2 = "Retrying on write timeout at consistency {0}(initial consistency: {1}, write type: {2}, required acknowledgments: {3}, received acknowledgments: {4}, retries: {5})";
                    Trace.TraceInformation(f2, CL(cl, decision), cl, writeType, requiredAcks, receivedAcks, nbRetry);
                    break;
            }
            return decision;
        }

        public RetryDecision OnUnavailable(ConsistencyLevel cl, int requiredReplica, int aliveReplica, int nbRetry)
        {
            RetryDecision decision = _policy.OnUnavailable(cl, requiredReplica, aliveReplica, nbRetry);
            switch (decision.DecisionType)
            {
                case RetryDecision.RetryDecisionType.Ignore:
                    const string f1 = "Ignoring unavailable exception (initial consistency: {0}, required replica: {1}, alive replica: {2}, retries: {3})";
                    Trace.TraceInformation(f1, cl, requiredReplica, aliveReplica, nbRetry);
                    break;
                case RetryDecision.RetryDecisionType.Retry:
                    const string f2 = "Retrying on unavailable exception at consistency {0} (initial consistency: {1}, required replica: {2}, alive replica: {3}, retries: {4})";
                    Trace.TraceInformation(f2, CL(cl, decision), cl, requiredReplica, aliveReplica, nbRetry);
                    break;
            }
            return decision;
        }
    }
}