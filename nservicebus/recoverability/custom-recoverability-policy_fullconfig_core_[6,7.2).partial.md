snippet: FullyCustomizedPolicyRecoverabilityConfiguration

The snippet below shows a fully custom policy that does the following:

 * For unrecoverable exceptions such as `MyBusinessException` failed messages are immediately moved to a custom error queue. 
 * For `MyOtherBusinessException` Delayed Retries are performed with a constant time increase of five seconds.
 * For all other cases failed messages are immediately moved to the configured error queue.

snippet: FullyCustomizedPolicy