
import time
from time import strftime, gmtime, localtime, strptime, sleep
import datetime
from bs4 import BeautifulSoup
import requests
import dicttoxml
import xml.etree.ElementTree as ET
import shelve
import os.path
import sys

sport = sys.argv[2]
scheduleDate = sys.argv[3]
sS=scheduleDate.split('/')
scheduleDate=sS[0]+'/'+sS[1]+'/20'+sS[2]
pathname = sys.argv[4]

currentTime = time.strftime('%I:%M %p', time.localtime())
currentDate = time.strftime('%m/%d/%Y', time.localtime())

scheduleFileList = []

i=0
url = ''
errorFlag='0'  
def scrape(url, i,errorFlag):

    gameList = []
    gametime = []
    awayTeams = []
    homeTeams = []
    teams=[]
    scores=[]
    clean={}
    gameTime = []
    scheduleTime = []
    gamecodes = []
    dayNum = i+1
    today = datetime.datetime.strptime(scheduleDate,'%m/%d/%Y')
    d = datetime.timedelta(days=i)  
    nextDay = today+d
    tomDateGame = datetime.datetime.strftime(nextDay,'%m/%d/%y')
    tomDateFile = datetime.datetime.strftime(nextDay,'%Y-%m-%d')
    url='http://sports.yahoo.com/'+ 'mlb'+'/scoreboard/?date=%(day)s' % {'day':tomDateFile}

    fileDate=strftime('%Y%m%d',localtime())
    dayOfWeek=strftime('%A',localtime())

    try:
        r=requests.get(url)
        soup=BeautifulSoup(r.content)
        homesoup = soup.find_all('tr', {'class': 'team home '})
        awaysoup = soup.find_all('tr', {'class': 'team away '})
        boxSoup = soup.find_all('td',{'class':'box'})
        timeSoup = BeautifulSoup(str(boxSoup)).find_all('span',{'class': 'time'})
        for players in homesoup:
            team = players.em.text
            team = team.replace('Chi', 'Chicago')
            homeTeams.append(team)
        
        for players in awaysoup:
            team = players.em.text
            team = team.replace('Chi', 'Chicago')
            awayTeams.append(team)
        
        for boxes in boxSoup:

            try:
                gamecode = boxes.a['href']
                gc1 =gamecode.split('/')
                gc2 = gc1[2].split('-')
                gamecode = gc2[len(gc2)-1]
                gamecodes.append(gamecode)
            except:
                pass
                
        for num in timeSoup:
            if 'span' in str(num):
                time = num.text
                time = time.replace(' EDT', '')
                scheduleTime.append(time)
            else:
                print 'Go back to your hole Yankees!'
                
    except:
        print 'Bad Dog'
        errorFlag='1'
        return gamecodes,scheduleTime,gameTime,awayTeams,homeTeams,gameList,dayNum,tomDateGame,errorFlag                  
    return gamecodes,scheduleTime,gameTime,awayTeams,homeTeams,gameList,dayNum,tomDateGame,errorFlag            

def dataScrub(scheduleTime,gameTime,awayTeams,homeTeams,gameList,errorFlag):
    errorFlag='0'
    for stat in scheduleTime:

        try:
            timeStamp = time.strptime(stat, '%I:%M %p')
            timeStamp = time.strftime('%H%M',timeStamp)
            hour = int(timeStamp[:2]) - 2
            mTime = str(hour) + timeStamp[-2:]
            mTime = time.strptime(mTime, '%H%M')
            timeStamp = time.strftime('%I:%M %p', mTime)
            if timeStamp[0] == '0':
                timeStamp = timeStamp[1:]   
            gameTime.append(timeStamp)
        except:
            gameTime.append('TBA')
            errorFlag='1'
        
    for i in range(len(scheduleTime)):
        num = 'game' + str(i + 1)
        gameList.append(num)

    return errorFlag
    
def makeSchedule(gameTime,homeTeams,awayTeams,tomDateGame,dayNum,scheduleFileList,scheduleTime,currentDate,currentTime,gamecodes,scheduleDate):  
    score = ET.Element('score')
    score.attrib['filedate'] = currentDate 
    score.attrib['filetime'] = currentTime
    print scheduleTime
    print str(len(scheduleTime))
    for i1 in range(len(scheduleTime)):
        game = ET.SubElement(score,'game')
        game.attrib['hometeam'] = homeTeams[i1]
        game.attrib['awayteam'] = awayTeams[i1]
        game.attrib['gamecode'] = gamecodes[i1] 
        homescore = ET.SubElement(game, 'homescore')
        homescore.text = '0'           
        awayscore = ET.SubElement(game, 'awayscore')
        awayscore.text = '0'            
        gametime = ET.SubElement(game, 'gametime')
        gametime.text = gameTime[i1]            
        gamedate = ET.SubElement(game, 'gamedate')
        gamedate.text = scheduleDate
        status = ET.SubElement(game, 'status')
        status.text ='Pre-Game'
        displaystatus1=ET.SubElement(game, 'display_status1')
        displaystatus1.text = ''
        displaystatus2=ET.SubElement(game, 'display_status2')
        displaystatus2.text = ''
    scheduleXML = ET.tostring(score)
    print scheduleXML
    scheduleFileName = 'ScheduleFile'+str(dayNum)+sys.argv[3]+'.xml'
    scheduleFileList.append(scheduleFileName)
        
    with open(pathname +'schedule' + sport + '.xml', 'w') as scoreData:
        scoreData.write(str(scheduleXML))

    return scheduleFileList
    

              
gamecodes,scheduleTime,gameTime,awayTeams,homeTeams,gameList,dayNum,tomDateGame,errorFlag = scrape(url,i,errorFlag)
print 'snake bite'
errorFlag=dataScrub(scheduleTime,gameTime,awayTeams,homeTeams,gameList,errorFlag)
                               
scheduleFileList=makeSchedule(gameTime,homeTeams,awayTeams,tomDateGame,dayNum,scheduleFileList,scheduleTime,currentDate,currentTime,gamecodes, scheduleDate)

