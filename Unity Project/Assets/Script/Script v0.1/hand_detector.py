import numpy as np
import cv2
import math
import socket
import time

from hand_detector_utils import *

UDP_IP = "127.0.0.1"
UDP_PORT = 5065

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

last = []

good_condition = False
drawing_box = True
full_frame = False
stabilize_highest_point = True

old_highest_point = (-1, -1)

x1_crop = 0
y1_crop = 60
x2_crop = 320
y2_crop = 420

# Open Camera
try:
    default = 1 # Try Changing it to 1 if webcam not found
    capture = cv2.VideoCapture(default)
except:
    print("No Camera Source Found!")

while capture.isOpened():
    
    # Capture frames from the camera
    ret, frame = capture.read()
    
    width = frame.shape[1]
                
    img_left = frame[y1_crop:y2_crop, 0:int(width/3)]
    img_right = frame[y1_crop:y2_crop, int(width/3 * 2): int(width)]
        
    try:

        contour_left = detectHand(img_left)
        contour_right = detectHand(img_right)
        
        defects_left, drawing_left = findDefects(img_left, contour_left)
        defects_right, drawing_right = findDefects(img_right, contour_right)
        
        # Count defects (in the right image)
        count_defects = countDefects(defects_right, contour_right, img_right)
        
        # Track highest point (in the left image)
        highest_point = trackHighestPoint(defects_left, contour_left)
        
        if(stabilize_highest_point):
            if( old_highest_point == (-1, -1)): old_highest_point = highest_point
            else:
                # Evaluate the magnitude of the difference
                diag_difference = np.linalg.norm(np.asarray(old_highest_point) - np.asarray(highest_point))
                
                # If the difference is bigger than a threshold then I actually moved my finger
                if(diag_difference >= 9.5): 
                    # print("diag_difference = ", diag_difference)
                    old_highest_point = highest_point
                else: highest_point = old_highest_point;
            
        if(full_frame):
            highest_point = (highest_point[0], highest_point[1])
            cv2.circle(frame, highest_point, 10, [255,0,255], -1)
        else:
            cv2.circle(img_left, highest_point, 10, [255,0,255], -1)
            highest_point = (highest_point[0] + x1_crop, highest_point[1] + y1_crop)
            cv2.circle(frame, highest_point, 10, [255,0,255], -1)
    
        # Print number of fingers
        textDefects(frame, count_defects,debug_var = False)
    
        # Show required images
        if(drawing_box):
            cv2.rectangle(frame, (x1_crop, y1_crop), (int(width/3), y2_crop),(0,0,255), 1)
            cv2.rectangle(frame, (int(width/3 * 2), y1_crop), (int(width), y2_crop),(0,0,255), 1)
        cv2.imshow("Full Frame", frame)
        
        all_image_left = np.hstack((drawing_left, img_left))
        cv2.imshow('Recognition Left', all_image_left)
        
        all_image_right = np.hstack((drawing_right, img_right))
        cv2.imshow('Recognition Right', all_image_right)
    
        last.append(count_defects)
        if(len(last) > 5):
            last = last[-5:]
            # last = []
        
    
        # Check if previously hand was wide open (3/4 fingers in previous frames), and is now a fist (0 fingers)
        if(good_condition):
            if(count_defects == 0 and 4 in last):
                last = []
                sendCommand(sock, UDP_IP, UDP_PORT, "ACTION")
                
            elif(count_defects == 0 and 2 in last):
                last = []
                sendCommand(sock, UDP_IP, UDP_PORT, "BACK")
                
        else:
            if(count_defects == 0 and 4 in last):
                last = []
                sendCommand(sock, UDP_IP, UDP_PORT, "ACTION")
        
        command = "l " + str(highest_point[0]) + " " + str(highest_point[1])
        
        
        sendCommand(sock, UDP_IP, UDP_PORT, command, debug_var = False)
        
    except:
        pass
    
    # Close the camera if 'q' is pressed
    if cv2.waitKey(1) == ord('q'):
        break

capture.release()
cv2.destroyAllWindows()