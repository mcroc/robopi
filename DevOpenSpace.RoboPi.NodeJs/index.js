const ws = require('ws');
const { delay, ServiceBusClient, ServiceBusMessage } = require("@azure/service-bus");

const connectionString = 'Endpoint=sb://devopenspacerobopi.servicebus.windows.net/;SharedAccessKeyName=DevOpenSpace;SharedAccessKey=TiPChIAXxLl4fH1qu+TXI7pDQLs7yqFAkGlZuSX2wR8=';
const topicName = 'robopicarnumber1';
const subscriptionName = 'robobicar1-pi';

async function main() {
    
    while(1 === 1) {
        var counter = 0;
        // create a Service Bus client using the connection string to the Service Bus namespace
        const sbClient = new ServiceBusClient(connectionString);

        // createReceiver() can also be used to create a receiver for a queue.
        const receiver = sbClient.createReceiver(topicName, subscriptionName);

        // function to handle messages
        const myMessageHandler = async (messageReceived) => {        
            console.log(`Received message: ${messageReceived.body}`);
            const sendWs = new ws.WebSocket("ws://10.10.10.229:8765");
            /*if(counter === 1) {
                sendWs.close();
                process.exit();
            }*/
            if(messageReceived.body == 'Start' && counter < 1) {
                counter = 1;
                var sendValue = {
                    'RC':'off',
                    'GS': "off",
                    'RD':'on',
                    'OA':'on',
                    'OF':'off',
                    'TL':['off',400],
                    'CD':['off',110],
                    'PW':50,
                    'SR':0,
                    'ST':'off',
                    'US':['off',0],
                    'MS':['off',4,0]
                };
                sendWs.onopen = function () {
                    console.log("require socket connect open...");
                    sendWs.send(JSON.stringify(sendValue));
                }            
            }
            if(messageReceived.body == 'Stopp') {
                counter = 1;
                var sendValue = {
                    'RC':'off',
                    'GS': "off",
                    'RD':'off',
                    'OA':'off',
                    'OF':'off',
                    'TL':['off',400],
                    'CD':['off',110],
                    'PW':50,
                    'SR':0,
                    'ST':'off',
                    'US':['off',0],
                    'MS':['off',4,0]
                };
                sendWs.onopen = function () {
                    console.log("require socket connect open...");
                    sendWs.send(JSON.stringify(sendValue));
                }            
            }
        };

        // function to handle any errors
        const myErrorHandler = async (error) => {
            console.log(error);
        };

        // subscribe and specify the message and error handlers
        receiver.subscribe({
            processMessage: myMessageHandler,
            processError: myErrorHandler
        });

        // Waiting long enough before closing the sender to send messages
        await delay(5000);

        await receiver.close();	
        await sbClient.close();
    }
}

// call the main function
main().catch((err) => {
	console.log("Error occurred: ", err);
	process.exit(1);
 });    