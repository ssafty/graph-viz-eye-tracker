from plugin import Plugin
import socket

# This file has to be copied to pupil/capture_settings/plugins/

class Tcp_Server(Plugin):
	"""
	Tcp Server broadcasts the gaze position
	over the network
	"""

	def __init__(self, g_pool):
		super(Tcp_Server, self).__init__(g_pool)
		self.order = .9
		self.ip = '127.0.0.1'
		self.port = 5006
		self.s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.s.bind((self.ip, self.port))
		self.s.listen(1)
		self.conn, addr = self.s.accept()

	def update(self,frame,events):		
		#pt['norm_pos']
		#for p in events.get('pupil_positions',[]):
		#    msg = "Pupil\n"
		#    for key,value in p.iteritems():
		#        if key not in self.exclude_list:
		#            msg +=key+":"+str(value)+'\n'
		#    self.socket.send( msg )

		for g in events.get('gaze_positions',[]):
			self.conn.send(str(g['norm_pos']))
			#for key, value in g.iteritems():
				#self.conn.send( key+":"+str(value)+'\n' )
			
		#for g in events.get('gaze_positions',[]):
		#	self.conn.send( g['norm_gaze'] )
		
		#self.conn.send("Hello, World")
		#conn.close()
