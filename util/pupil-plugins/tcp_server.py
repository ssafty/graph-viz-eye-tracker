from plugin import Plugin
import socket
import os
from pyglui import ui

# This file has to be copied to pupil/capture_settings/plugins/

class Tcp_Server(Plugin):
    """
    Tcp Server broadcasts the gaze position
    over the network
    """

    def __init__(self, g_pool):
        super(Tcp_Server, self).__init__(g_pool)
        self.order = .9
        self.ip = '0.0.0.0'
        self.port = 5006
        self.s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.s.bind((self.ip, self.port))
        self.s.listen(1)
        self.conn, addr = self.s.accept()


    def init_gui(self):
        if self.g_pool.app == 'capture':
            self.menu = ui.Growing_Menu("TCP Broadcast Server")
            self.g_pool.sidebar.append(self.menu)

        self.menu.append(ui.Button('Close', self.close))
        help_str = "TCP Message server"
        self.menu.append(ui.Info_Text(help_str))
    #self.menu.append(ui.Text_Input('address', self, setter=self.set_server, label='Address'))


    def deinit_gui(self):
        if self.menu and self.g_pool.app == 'capture':
            self.g_pool.sidebar.remove(self.menu)
        self.menu = None

    def close(self): self.alive = False


    def get_init_dict(self): return {'address': self.address}


    def cleanup(self):
        """gets called when the plugin get terminated.
        This happens either voluntarily or forced.
        """
        self.deinit_gui()
        self.conn.close() #close connection!
        self.s.close() #close socket


    def update(self,frame,events):
        # pf = open('pupil_pos', 'a')
        # for p in events.get('pupil_positions',[]):
        #     try:
        #         pf.write(p + '\n')
        #     except:
        #         print "no pupil position"
        # msg = "Pupil\n"
        # for key,value in p.iteritems():
        #     if key not in self.exclude_list:
        #         msg +=key+":"+str(value)+'\n'
        # self.socket.send( msg )
        # gf = open('pupil_pos', 'a')
        for g in events.get('gaze_positions',[]):
            #self.conn.send(str(g['norm_pos']))
            #print g
            try:
                print str(g['realtime gaze on unnamed'])
                self.conn.send(str(g['realtime gaze on unnamed']))
                gf.write(g + '\n')
            except:
                print "no realtime"
    #for key, value in g.iteritems():
    #self.conn.send( key+":"+str(value)+'\n' )

    #for g in events.get('gaze_positions',[]):
    #   self.conn.send( g['norm_gaze'] )

    #self.conn.send("Hello, World")
    #conn.close()

