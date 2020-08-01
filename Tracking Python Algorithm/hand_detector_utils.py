import numpy as np
import cv2
import math
import socket
import time

UDP_IP = "127.0.0.1"
UDP_PORT = 5065

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)


def detectHand(img, kernel_dim = (5,5)):
    # Apply Gaussian blur
    blur = cv2.GaussianBlur(img, (3,3), 0)
    
    # Change color-space from BGR -> HSV
    hsv = cv2.cvtColor(blur, cv2.COLOR_BGR2HSV)
        
    # Create a binary image with where white will be skin colors and rest is black
    mask2 = cv2.inRange(hsv, np.array([2, 0, 0]), np.array([20, int(255 * 0.68), 255]))
    
    # Kernel for morphological transformation    
    kernel = np.ones(kernel_dim)
    
    # Apply morphological transformations to filter out the background noise
    dilation = cv2.dilate(mask2, kernel, iterations = 2)
    erosion = cv2.erode(dilation, kernel, iterations = 2)    
       
    # Apply Gaussian Blur and Threshold (To clean)
    filtered = cv2.GaussianBlur(erosion, (3,3), 0)
    ret, thresh = cv2.threshold(filtered, 127, 255, 0)
    
    # cv2.imshow('filtered', filtered)
    # cv2.imshow('thresh - filtered', thresh - filtered)
    cv2.imshow('thresh', thresh)
    
    try:   
        # Find contours
        contours, hierarchy = cv2.findContours(thresh, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE )
        
        # Find biggest contour
        contour = max(contours, key = lambda x: cv2.contourArea(x))
       
        return contour
    except:
        
        return np.zeros(0)


def findDefects(crop_image, contour):
    # Create bounding rectangle around the contour
    x,y,w,h = cv2.boundingRect(contour)
    cv2.rectangle(crop_image, (x,y), (x+w,y+h), (255,0,255), 0)
    
    # Find convex hull
    hull = cv2.convexHull(contour)
    
    # Draw contour
    drawing = np.zeros(crop_image.shape, np.uint8)
    cv2.drawContours(drawing, [contour], -1, (0,0,255), 0)
    cv2.drawContours(drawing, [hull], -1, (0,255,0), 0)
    
    # Find convexity defects
    hull = cv2.convexHull(contour, returnPoints = False)
    defects = cv2.convexityDefects(contour, hull)
    
    return defects, drawing

def countDefects(defects, contour, crop_image):
    count_defects = 0
    for i in range(defects.shape[0]):
        # if(i == 0): print(defects[i,0])
        
        s, e, f, d = defects[i,0]
        start = tuple(contour[s][0])
        end = tuple(contour[e][0])
        far = tuple(contour[f][0])

        a = math.sqrt((end[0] - start[0])**2 + (end[1] - start[1])**2)
        b = math.sqrt((far[0] - start[0])**2 + (far[1] - start[1])**2)
        c = math.sqrt((end[0] - far[0])**2 + (end[1] - far[1])**2)
        angle = (math.acos((b**2 + c**2 - a**2)/(2*b*c))*180)/3.14
        
        # if angle < 90 draw a circle at the far point
        if angle <= 90:
            count_defects += 1
            cv2.circle(crop_image, start, 10, [255,0,0], -1)
            cv2.circle(crop_image, end, 10, [0,255,0], -1)
            cv2.circle(crop_image, far, 10, [0,0,255], -1)
            
        cv2.line(crop_image, start, end, [0,255,0], 2)
        
    return count_defects

def trackHighestPoint(defects, contour):
    # Tracking of the highest point detected
    highest_point = (1920, 1080)
    
    for i in range(defects.shape[0]):
        # if(i == 0): print(defects[i,0])
        
        s,e,f,d = defects[i,0]
        tmp_point = tuple(contour[s][0])
        
        if(tmp_point[1] < highest_point[1]): highest_point = tmp_point;
        
    return highest_point


def textDefects(frame, count_defects,  color = [255,0,255], debug_var = False):
    if(debug_var): print("Defects : ", count_defects)
   
    if count_defects == 0:
        cv2.putText(frame,"ZERO", (50,50), cv2.FONT_HERSHEY_SIMPLEX, 2, color)
    elif count_defects == 1:
        cv2.putText(frame,"TWO", (50,50), cv2.FONT_HERSHEY_SIMPLEX, 2, color)
    elif count_defects == 2:
        cv2.putText(frame, "THREE", (5,50), cv2.FONT_HERSHEY_SIMPLEX, 2, color)
    elif count_defects == 3:
        cv2.putText(frame,"FOUR", (50,50), cv2.FONT_HERSHEY_SIMPLEX, 2, color)
    elif count_defects == 4:
        cv2.putText(frame,"FIVE", (50,50), cv2.FONT_HERSHEY_SIMPLEX, 2, color)
    else:
        pass
    
def sendCommand(sock, UDP_IP, UDP_PORT, command, debug_var = True):
    sock.sendto((command).encode(), (UDP_IP, UDP_PORT) )
    
    if(debug_var): print("_"*10, command, " sent!", "_"*10)
    
    
def detectFace(frame, print_var = False):
    # Load the cascade
    face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')
    
    # Convert into grayscale
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    # Detect faces
    faces = face_cascade.detectMultiScale(gray, 1.1, 4)
    
    # Find mean value of H channel
    x, y, w, h = faces[0]
    face_img = frame[y:(y + h), x:(x + w)]
    
    face_img = cv2.cvtColor(face_img, cv2.COLOR_BGR2HSV)
    face_img = cv2.inRange(face_img, np.array([2,0,0]), np.array([20,255,255]))
    
    # Display the output
    if(print_var): 
        # Frame copy
        frame_copy = frame.copy()
        
        # Draw rectangle around the faces
        for (x, y, w, h) in faces:
            cv2.rectangle(frame_copy, (x, y), (x+w, y+h), (255, 0, 0), 2)
        
        # cv2.imshow('Face Position', frame_copy)
        cv2.imshow('Face face_img', face_img)
        

def detectHandV2(frame, net):
    nPoints = 22
    POSE_PAIRS = [ [0,1],[1,2],[2,3],[3,4],[0,5],[5,6],[6,7],[7,8],[0,9],[9,10],[10,11],[11,12],[0,13],[13,14],[14,15],[15,16],[0,17],[17,18],[18,19],[19,20] ]
    
    threshold = 0.2
    
    frameWidth = frame.shape[1]
    frameHeight = frame.shape[0]
    
    aspect_ratio = frameWidth/frameHeight
    
    inHeight = 768
    inWidth = int(((aspect_ratio*inHeight)*8)//8)

    # frameCopy = np.copy(frame)
    frameCopy = frame

    inpBlob = cv2.dnn.blobFromImage(frame, 1.0 / 255, (inWidth, inHeight), (0, 0, 0), swapRB=False, crop=False)
    # inpBlob = cv2.dnn.blobFromImage(frame)
    
    net.setInput(inpBlob)

    output = net.forward()

    # Empty list to store the detected keypoints
    points = []

    for i in range(nPoints):
        # confidence map of corresponding body's part.
        probMap = output[0, i, :, :]
        probMap = cv2.resize(probMap, (frameWidth, frameHeight))

        # Find global maxima of the probMap.
        minVal, prob, minLoc, point = cv2.minMaxLoc(probMap)

        if prob > threshold :
            cv2.circle(frameCopy, (int(point[0]), int(point[1])), 6, (0, 255, 255), thickness=-1, lineType=cv2.FILLED)
            cv2.putText(frameCopy, "{}".format(i), (int(point[0]), int(point[1])), cv2.FONT_HERSHEY_SIMPLEX, .8, (0, 0, 255), 2, lineType=cv2.LINE_AA)

            # Add the point to the list if the probability is greater than the threshold
            points.append((int(point[0]), int(point[1])))
        else :
            points.append(None)

    # # Draw Skeleton
    # for pair in POSE_PAIRS:
    #     partA = pair[0]
    #     partB = pair[1]

    #     if points[partA] and points[partB]:
    #         cv2.line(frame, points[partA], points[partB], (0, 255, 255), 2, lineType=cv2.LINE_AA)
    #         cv2.circle(frame, points[partA], 5, (0, 0, 255), thickness=-1, lineType=cv2.FILLED)
    #         cv2.circle(frame, points[partB], 5, (0, 0, 255), thickness=-1, lineType=cv2.FILLED)


    # cv2.putText(frame, "time taken = {:.2f} sec".format(time.time() - t), (50, 50), cv2.FONT_HERSHEY_COMPLEX, .8, (255, 50, 0), 2, lineType=cv2.LINE_AA)
    # cv2.putText(frame, "Hand Pose using OpenCV", (50, 50), cv2.FONT_HERSHEY_COMPLEX, 1, (255, 50, 0), 2, lineType=cv2.LINE_AA)
    cv2.imshow('Output-Skeleton', frame)
