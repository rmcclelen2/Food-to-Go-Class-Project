import React, { useState, useEffect } from "react";
import axios from "axios";
import { Table, Container } from "semantic-ui-react";
import StaffNavbar from "../Components/StaffNavbar";

//this creates an array of current 2 weeks
var currentDate = new Date();
let date = new Date(
    currentDate.setDate(currentDate.getDate() - currentDate.getDay())
  ),
  date1 = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 14),
  dateArray = [];
for (let q = date; q <= date1; q.setDate(q.getDate() + 1)) {
  dateArray.push(q.toDateString());
}

let firstweek = dateArray.slice(0, 7);
let secondweek = dateArray.slice(7, 14);

export default function Schedule() {
  const [userData, setUserData] = useState([]);

  useEffect(() => {
    axios.get("api/authentication/me").then(response => {
      setUserData(response.data);
    });
  }, []);

  return (
    <>
      <StaffNavbar />

      <div>
        <Container>
          <Table celled>
            <Table.Header>
              <Table.Row>
                <Table.HeaderCell width={2}>Name</Table.HeaderCell>
                {firstweek.map(table => (
                  <Table.HeaderCell width={1}>{table}</Table.HeaderCell>
                ))}
              </Table.Row>
            </Table.Header>
            <Table.Body>
              <Table.Row>
                <Table.Cell>{userData.name}</Table.Cell>
                {userData?.schedule
                  ? userData.schedule
                      .slice(0, 7)
                      .map(s => (
                        <Table.Cell key={s.dailySchedule}>{s.dailySchedule}</Table.Cell>
                      ))
                  : null}
              </Table.Row>
            </Table.Body>
          </Table>
          <Table celled>
            <Table.Header>
              <Table.Row>
                <Table.HeaderCell width={2}>Name</Table.HeaderCell>
                {secondweek.map(table => (
                  <Table.HeaderCell width={1}>{table}</Table.HeaderCell>
                ))}
              </Table.Row>
            </Table.Header>
            <Table.Body>
              <Table.Row>
                <Table.Cell>{userData.name}</Table.Cell>
                {userData?.schedule
                  ? userData.schedule
                      .slice(7, 14)
                      .map(s => (
                        <Table.Cell key={s.dailySchedule}>{s.dailySchedule}</Table.Cell>
                      ))
                  : null}
              </Table.Row>
            </Table.Body>
          </Table>
        </Container>
      </div>
    </>
  );
}
