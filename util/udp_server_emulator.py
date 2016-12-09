__author__ = 'saftophobia'
import numpy as np
import socket
import time

IP = '127.0.0.1'
PORT = 5007

print "Welcome to UDP Server Emulator v0.1"
print "Emulating a server on " + str(IP) + ":" + str(PORT)
print "Please press Control+C when you no longer need the emulator running ... "

server = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

def broadcast_frequency(x, y, count_per_second):
    for i in range(0, count_per_second):
        x_noised, y_noised = str(x + np.random.normal(0, 0.1)), str(y + np.random.normal(0, 0.1))

        #print "Sending %s,%s"   %(x_noised, y_noised)
        server.sendto("(%s,%s)" %(x_noised, y_noised), (IP, PORT))
        time.sleep(1/count_per_second)

while True:
    x = np.random.random()
    y = np.random.random()

    broadcast_frequency(x, y, count_per_second= 30)

