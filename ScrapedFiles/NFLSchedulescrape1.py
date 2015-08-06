import time
from time import strftime, gmtime, localtime, strptime, sleep
import datetime
from bs4 import BeautifulSoup
import requests
import dicttoxml
from xml.etree.ElementTree import *
import shelve
import os.path
import sys

seasonStart = sys.argv[4]
seasonEnd = sys.argv[5]
sS=seasonStart.split('/')
sE= seasonEnd.split('/')
seasonStart=sS[0]+'/'+sS[1]+'/20'+sS[2]
#seasonEnd=sE[0]+'/'+sE[1]+'/20'+sE[2]

scheduleFileList = []

for i in range(0,180,1):
    url = ''
    errorFlag='0'
    print i
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
        dayNum = i+1
        today = datetime.datetime.strptime(seasonStart,'%m/%d/%Y')
        d = datetime.timedelta(days=i)  
        nextDay = today+d
        tomDateGame = datetime.datetime.strftime(nextDay,'%m/%d/%y')
        tomDateFile = datetime.datetime.strftime(nextDay,'%Y-%m-%d')
        print 'tomDateFile = ' + str(tomDateFile)
        url='http://sports.yahoo.com/'+ sys.argv[1]+'/scoreboard/?date=%(day)s' % {sys.argv[2]:tomDateFile}
        currentTime=strftime('%H%M',localtime())
        fileDate=strftime('%Y%m%d',localtime())
        dayOfWeek=strftime('%A',localtime())
        try:
            r=requests.get(url)
            soup=BeautifulSoup(r.content)
            homesoup = soup.find_all('tr', {'class': 'team home '})
            awaysoup = soup.find_all('tr', {'class': 'team away '})
            timeSoup = soup.find_all('span',{'class': 'time'})
            for players in homesoup:
                team = players.em.text
                team = team.replace('Chi', 'Chicago')
                homeTeams.append(team)
        
            for players in awaysoup:
                team = players.em.text
                team = team.replace('Chi', 'Chicago')
                awayTeams.append(team)
            
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
            return scheduleTime,gameTime,awayTeams,homeTeams,gameList,dayNum,tomDateGame,errorFlag                  
        return scheduleTime,gameTime,awayTeams,homeTeams,gameList,dayNum,tomDateGame,errorFlag            

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
    
    def makeSchedule(gameTime,homeTeams,awayTeams,tomDateGame,dayNum,scheduleFileList):
        schedule=Element('schedule')
        TimeP = SubElement(schedule,'TimeP')
        TimeP.attrib['TimePeriod'] = 'Day' + str(dayNum)
        TimeP.attrib['Sport'] = sys.argv[3]
        for i in range(len(scheduleTime)):
            game = Element('game')
            game.attrib['gameNumber']=gameList[i]
            hometeam = SubElement(game, 'homeTeam')
            hometeam.text = homeTeams[i]
            awayteam = SubElement(game, 'awayTeam')
            awayteam.text = awayTeams[i]
            gametime = SubElement(game, 'startTime')
            gametime.text = gameTime[i]            
            startdate = SubElement(game, 'startDate')
            startdate.text = tomDateGame
            TimeP.insert(i,game)
        scheduleXML = tostring(schedule)
        scheduleFileName = 'ScheduleFile'+str(dayNum)+sys.argv[3]+'.xml'
        scheduleFileList.append(scheduleFileName)
        
        with open(scheduleFileName,'w') as scoreData:
            scoreData.write(str(scheduleXML))

        return scheduleFileList
    
    def makeScheduleFileListFile(scheduleFileList):
        scheduleFiles = Element('schedulefiles')
        scheduleFiles.attrib['Sport']=sys.argv[3]

        for i in range(len(scheduleFileList)):
         
            file1=Element('file')
            file1.attrib['fileNumber']= 'file'+ str(i)
            filePath1=SubElement(file1,'filepath')
            filePath1.text='.\\' + scheduleFileList[i]
            scheduleFiles.insert(i,filePath1)

        scheduleFileListXML=tostring(scheduleFiles)    
        scheduleFileListName = 'ScheduleFileList' + sys.argv[3]+'.xml' 

        with open(scheduleFileListName,'w') as scheduleList:
            scheduleList.write(str(scheduleFileListXML))

              
    scheduleTime,gameTime,awayTeams,homeTeams,gameList,dayNum,tomDateGame,errorFlag = scrape(url,i,errorFlag)

    errorFlag=dataScrub(scheduleTime,gameTime,awayTeams,homeTeams,gameList,errorFlag)

                               
    scheduleFileList=makeSchedule(gameTime,homeTeams,awayTeams,tomDateGame,dayNum,scheduleFileList)

    makeScheduleFileListFile(scheduleFileList)
    
    print tomDateGame
    dt = datetime.datetime.strptime(seasonEnd,'%m/%d/%y')
    seasonEnd=dt.strftime('%m/%d/%y')
    print seasonEnd
    if tomDateGame==seasonEnd:
        break

    print errorFlag
    if errorFlag=='1':
        print gameTime
        
    time.sleep(5)
