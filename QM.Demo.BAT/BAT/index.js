const puppeteer = require('puppeteer');

(async () => {
  const browser = await puppeteer.launch({headless:false});
  const page = await browser.newPage();
  await page.setViewport({width:1000,height:800});
  //打开指定的网站
  await page.goto('http://10.68.10.109');  
  console.log('打开指定的网站');
  //待指定内容加载完成
  await page.waitForSelector('a');
  //触发点击事件
  await page.click('a');  
  //输入帐号
  await page.waitForSelector('#username');
  await page.focus('#username');
  await page.keyboard.type('111');
  //输入密码
  await page.waitForSelector('#password');
  await page.focus('#password');
  await page.keyboard.type('111');
  //点击登陆
  await page.click('button[type=submit]');
  console.log('登陆完成');
  
  //iframe
  await page.waitFor('frameset#demo');
  const frames = await page.frames();
  console.log(frames.length);  
  const topframe = await frames.find(f => f.name() === 'banner');
  await topframe.waitFor('#search');
  console.log('加载frame完成');
    
  //输入报表名称
  await topframe.type('#search','wip');
  await topframe.click('#searchBtn');

  //报表指定的动作
  await page.waitFor('frame#main');
  console.log(frames.length);  
  const mainframe = await frames.find(f => f.name() === 'main');
  await mainframe.waitFor('select#CustomerKey');  
  await mainframe.select('select#CustomerKey','CRE','STM');
  await mainframe.click('input#rbl_type_1');
  await mainframe.click('input#Button_Query');
  
  //等待响应完成

  
  //截图
  //await page.screenshot({path: '109.png'});
  
  //关闭浏览器
  //await browser.close();
})();